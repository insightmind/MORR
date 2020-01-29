using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using MORR.Core.Data.Capture.Video;
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

            Task.Run(ConsumeVideoSamples);
            EncodeInternalAsync(recordingFilePath); // Intentionally do not block here
        }

        private FilePath GetRecordingFile(DirectoryPath recordingDirectoryPath)
        {
            var recordingFilePath = Path.Combine(recordingDirectoryPath.ToString(), $"{Configuration.RecordingName}.mp4");
            var recordingFile = new FilePath(recordingFilePath);
            return recordingFile;
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

        private async Task EncodeInternalAsync(FilePath destination)
        {
            var transcoder = GetTranscoder();

            var (videoStreamDescriptor, metadataStreamDescriptor) = GetStreamDescriptors();

            var mediaStreamSource = GetMediaStreamSource(videoStreamDescriptor, metadataStreamDescriptor);
            var encodingProfile = GetEncodingProfile();

            await using var destinationFile = File.Open(destination.ToString(), FileMode.Create);

            var prepareTranscodeResult =
                await transcoder.PrepareMediaStreamSourceTranscodeAsync(
                    mediaStreamSource, destinationFile.AsRandomAccessStream(), encodingProfile);
            await prepareTranscodeResult.TranscodeAsync().AsTask();
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

        private Tuple<VideoStreamDescriptor, TimedMetadataStreamDescriptor> GetStreamDescriptors()
        {
            var videoEncoding = VideoEncodingProperties.CreateUncompressed(MediaEncodingSubtypes.Bgra8,
                                                                           Configuration.InputWidth,
                                                                           Configuration.InputHeight);

            var videoStreamDescriptor = new VideoStreamDescriptor(videoEncoding)
            {
                Name = "Desktop video stream",
                Label = "Desktop video stream"
            };

            var metadataEncoding = new TimedMetadataEncodingProperties
            {
                Subtype = "{36002D6F-4D0D-4FD7-8538-5680DA4ED58D}"
            };

            // Arbitrary byte sequence to uniquely identify MORR data
            byte[] streamDescriptionData =
            {
                0x4d, 0x4f, 0x52, 0x52,
                0x45, 0x76, 0x65, 0x6e,
                0x74, 0x54, 0x72, 0x61,
                0x63, 0x6b, 0xFF, 0xFF
            };

            metadataEncoding.SetFormatUserData(streamDescriptionData);

            var metadataStreamDescriptor = new TimedMetadataStreamDescriptor(metadataEncoding)
            {
                Name = "MORR Event data",
                Label = "MORR Event data"
            };

            return new Tuple<VideoStreamDescriptor, TimedMetadataStreamDescriptor>(
                videoStreamDescriptor, metadataStreamDescriptor);
        }

        private MediaStreamSource GetMediaStreamSource(IMediaStreamDescriptor videoStreamDescriptor,
                                                       IMediaStreamDescriptor metadataStreamDescriptor)
        {
            var mediaStreamSource = new MediaStreamSource(videoStreamDescriptor, metadataStreamDescriptor)
            {
                BufferTime = TimeSpan.FromSeconds(0)
            };

            mediaStreamSource.Starting += OnStart;
            mediaStreamSource.SampleRequested += OnSampleRequested;

            return mediaStreamSource;
        }

        private MediaTranscoder GetTranscoder()
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

            // TODO Metadata encoding / handle different StreamDescriptor requests
            // This method is know to never be called from a TimedMetadataStreamDescriptor, so we can skip handling this case for now
        }

        #endregion
    }
}