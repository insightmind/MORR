using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using MORR.Core.Data.Capture.Video;
using MORR.Core.Data.Transcoding.Exceptions;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Core.Data.Transcoding.Mpeg
{
    public class MpegEncoder : IEncoder
    {
        private readonly AutoResetEvent nextSampleReady = new AutoResetEvent(false);
        private readonly ManualResetEvent onResolutionInferred = new ManualResetEvent(false);
        private readonly AutoResetEvent sampleProcessed = new AutoResetEvent(true);
        private DateTime encodingStart;
        private Tuple<uint, uint>? inferredResolution;
        private DirectXVideoSample? nextSample;

        [Import]
        private MpegEncoderConfiguration Configuration { get; set; }

        [Import]
        private IEncodeableEventQueue<DirectXVideoSample> VideoQueue { get; set; }

        public ManualResetEvent EncodeFinished { get; } = new ManualResetEvent(false);

        public void Encode(DirectoryPath recordingDirectoryPath)
        {
            encodingStart = DateTime.Now;

            Task.Run(ConsumeVideoSamples);
            Task.Run(() => InitializeTranscode(recordingDirectoryPath));
        }

        private FileStream GetFileStream(DirectoryPath recordingDirectoryPath)
        {
            var fullPath = Path.Combine(recordingDirectoryPath.ToString(), Configuration.RelativeFilePath.ToString());
            return File.OpenWrite(fullPath);
        }

        private async void InitializeTranscode(DirectoryPath recordingDirectoryPath)
        {
            onResolutionInferred.WaitOne();

            if (inferredResolution == null)
            {
                throw new EncodingException("Failed to infer video input resolution.");
            }

            var transcoder = GetTranscoder();
            var streamDescriptor = GetStreamDescriptor(inferredResolution.Item1, inferredResolution.Item2);

            var mediaStreamSource = GetMediaStreamSource(streamDescriptor);
            var encodingProfile = GetEncodingProfile();

            await using var destinationFile = GetFileStream(recordingDirectoryPath);

            EncodeFinished.Reset();
            var prepareTranscodeResult =
                await transcoder.PrepareMediaStreamSourceTranscodeAsync(
                    mediaStreamSource, destinationFile.AsRandomAccessStream(), encodingProfile);

            if (!prepareTranscodeResult.CanTranscode)
            {
                throw new EncodingException(
                    $"Failed to start transcoding operation. Reason: {prepareTranscodeResult.FailureReason}");
            }

            await prepareTranscodeResult.TranscodeAsync().AsTask().ContinueWith(_ => EncodeFinished.Set());
        }

        private async Task ConsumeVideoSamples()
        {
            await foreach (var videoSample in VideoQueue.GetEvents())
            {
                if (inferredResolution == null)
                {
                    // DesktopCapture may enqueue null samples intentionally to stop encoding
                    if (videoSample == null)
                    {
                        throw new EncodingException("Failed to infer video input resolution.");
                    }

                    var description = videoSample.Surface.Description;
                    var inferredWidth = (uint) description.Width;
                    var inferredHeight = (uint) description.Height;

                    inferredResolution = new Tuple<uint, uint>(inferredWidth, inferredHeight);
                    onResolutionInferred.Set();
                }

                sampleProcessed.WaitOne();
                nextSample = videoSample;
                nextSampleReady.Set();
            }

            // Stop encoding by sending null sample
            sampleProcessed.WaitOne();
            nextSample = null;
            nextSampleReady.Set();
        }

        #region Transcoder setup

        private MediaEncodingProfile GetEncodingProfile()
        {
            var profile = new MediaEncodingProfile();
            var containerEncoding = new ContainerEncodingProperties
            {
                Subtype = MediaEncodingSubtypes.Mpeg4
            };

            const uint bitsPerKiloBit = 1000;

            var videoEncoding = new VideoEncodingProperties
            {
                Subtype = MediaEncodingSubtypes.H264,
                Width = Configuration.Width,
                Height = Configuration.Height,
                Bitrate = Configuration.KiloBitsPerSecond * bitsPerKiloBit,
                FrameRate = { Denominator = 1, Numerator = Configuration.FramesPerSecond },
                PixelAspectRatio = { Denominator = 1, Numerator = 1 }
            };

            profile.Container = containerEncoding;
            profile.Video = videoEncoding;

            return profile;
        }

        private static VideoStreamDescriptor GetStreamDescriptor(uint width, uint height)
        {
            var videoEncoding = VideoEncodingProperties.CreateUncompressed(MediaEncodingSubtypes.Bgra8,
                                                                           width, height);

            var videoStreamDescriptor = new VideoStreamDescriptor(videoEncoding)
            {
                Name = "Desktop video stream",
                Label = "Desktop video stream"
            };

            return videoStreamDescriptor;
        }

        private MediaStreamSource GetMediaStreamSource(IMediaStreamDescriptor videoStreamDescriptor)
        {
            var mediaStreamSource = new MediaStreamSource(videoStreamDescriptor)
            {
                BufferTime = TimeSpan.FromSeconds(0)
            };

            mediaStreamSource.Starting += OnStart;
            mediaStreamSource.SampleRequested += OnSampleRequested;

            return mediaStreamSource;
        }

        private static MediaTranscoder GetTranscoder()
        {
            var transcoder = new MediaTranscoder { HardwareAccelerationEnabled = true };
            return transcoder;
        }

        #endregion

        #region Event handlers

        private void OnStart(MediaStreamSource sender, MediaStreamSourceStartingEventArgs args)
        {
            args.Request.SetActualStartPosition(DateTime.Now - encodingStart);
        }

        private void OnSampleRequested(MediaStreamSource sender, MediaStreamSourceSampleRequestedEventArgs args)
        {
            if (args.Request.StreamDescriptor is VideoStreamDescriptor)
            {
                var deferral = args.Request.GetDeferral();
                nextSampleReady.WaitOne();

                if (nextSample != null)
                {
                    args.Request.Sample =
                        MediaStreamSample.CreateFromDirect3D11Surface(nextSample.Surface,
                                                                      nextSample.Timestamp - encodingStart);
                    // Manually forcing the surface to be disposed helps with memory consumption
                    // As this is the only consumer, this is safe
                    nextSample.Surface.Dispose();
                }
                else
                {
                    args.Request.Sample = null;
                }

                sampleProcessed.Set();
                deferral.Complete();
            }
        }

        #endregion
    }
}