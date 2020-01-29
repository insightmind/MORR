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

namespace MORR.Core.Data.Transcoding.MPEG
{
    public class MPEGEncoder : IEncoder
    {
        private readonly AutoResetEvent nextSampleReady = new AutoResetEvent(false);
        private readonly AutoResetEvent sampleProcessed = new AutoResetEvent(true);
        private DateTime encodingStart;
        private DirectXVideoSample? nextSample;

        [Import]
        private MPEGEncoderConfiguration Configuration { get; set; }

        [Import]
        private IEncodeableEventQueue<DirectXVideoSample> VideoQueue { get; set; }

        public void Encode(DirectoryPath recordingDirectoryPath)
        {
            encodingStart = DateTime.Now;
            var recordingFilePath = GetRecordingFile(recordingDirectoryPath);

            Task.Run(GetFirstSample).ContinueWith(x => InitializeTranscode(x.Result, recordingFilePath));
        }

        private FilePath GetRecordingFile(DirectoryPath recordingDirectoryPath)
        {
            var recordingFilePath =
                Path.Combine(recordingDirectoryPath.ToString(), $"{Configuration.RecordingName}.mp4");
            var recordingFile = new FilePath(recordingFilePath);
            return recordingFile;
        }

        private async void InitializeTranscode(DirectXVideoSample firstSample, FilePath recordingFilePath)
        {
            ConsumeVideoSamples();

            var width = (uint) firstSample.Surface.Description.Width;
            var height = (uint) firstSample.Surface.Description.Height;

            var transcoder = GetTranscoder();
            var streamDescriptor = GetStreamDescriptor(width, height);

            var mediaStreamSource = GetMediaStreamSource(streamDescriptor);
            var encodingProfile = GetEncodingProfile();

            await using var destinationFile = File.Open(recordingFilePath.ToString(), FileMode.Create);

            var prepareTranscodeResult =
                await transcoder.PrepareMediaStreamSourceTranscodeAsync(
                    mediaStreamSource, destinationFile.AsRandomAccessStream(), encodingProfile);

            if (!prepareTranscodeResult.CanTranscode)
            {
                throw new EncodingException();
            }

            await prepareTranscodeResult.TranscodeAsync();
        }

        private async Task<DirectXVideoSample> GetFirstSample()
        {
            await foreach (var videoSample in VideoQueue.GetEvents())
            {
                return videoSample;
            }

            return null;
        }

        private async Task ConsumeVideoSamples()
        {
            await foreach (var videoSample in VideoQueue.GetEvents())
            {
                sampleProcessed.WaitOne();
                nextSample = videoSample;
                nextSampleReady.Set();
            }
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