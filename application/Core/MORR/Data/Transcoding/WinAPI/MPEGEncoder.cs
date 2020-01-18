using System;
using System.Composition;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using MORR.Shared.Utility;

namespace MORR.Core.Data.Transcoding.WinAPI
{
    [Export(typeof(IEncoder))]
    public class MPEGEncoder : IEncoder
    {
        [Import]
        private MPEGEncoderConfiguration Configuration { get; set; }

        private MediaEncodingProfile GetEncodingProfile(VideoStreamDescriptor videoStreamDescriptor,
                                                        TimedMetadataStreamDescriptor metadataStreamDescriptor)
        {
            var profile = new MediaEncodingProfile();
            var containerEncoding = new ContainerEncodingProperties
            {
                Subtype = MediaEncodingSubtypes.Mpeg4
            };

            var videoEncoding = new VideoEncodingProperties
            {
                Subtype = MediaEncodingSubtypes.H264,
                Width = Configuration.Width,
                Height = Configuration.Height,
                Bitrate = Configuration.BitsPerSecond,
                FrameRate = { Denominator = 1, Numerator = Configuration.FramesPerSecond },
                PixelAspectRatio = { Denominator = 1, Numerator = 1 }
            };

            profile.Container = containerEncoding;
            profile.Video = videoEncoding;

            // Setting the video track explicitly on the profile causes a NullReferenceException for an unknown reason
            //profile.SetVideoTracks(new[] { videoStreamDescriptor.Copy() });
            //profile.SetTimedMetadataTracks(new[] { metadataStreamDescriptor.Copy() });

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

        public async Task EncodeAsync(FilePath destination)
        {
            var transcoder = GetTranscoder();

            var (videoStreamDescriptor, metadataStreamDescriptor) = GetStreamDescriptors();

            var mediaStreamSource = GetMediaStreamSource(videoStreamDescriptor, metadataStreamDescriptor);
            var encodingProfile = GetEncodingProfile(videoStreamDescriptor, metadataStreamDescriptor);

            await using var destinationFile = File.Open(destination.ToString(), FileMode.Create);

            var prepareTranscodeResult =
                await transcoder.PrepareMediaStreamSourceTranscodeAsync(
                    mediaStreamSource, destinationFile.AsRandomAccessStream(), encodingProfile);
            await prepareTranscodeResult.TranscodeAsync().AsTask();
        }

        public void Encode()
        {
            var currentTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            var recordingFilePath =
                new FilePath(Path.Combine(Configuration.RecordingsDirectory.ToString(), $"{currentTime}.mp4"));
            var encodeTask = EncodeAsync(recordingFilePath);
            encodeTask.GetAwaiter().GetResult();
        }

        #region Event handlers

        private void OnStart(MediaStreamSource sender, MediaStreamSourceStartingEventArgs args)
        {
            // This gets called once on initialization and does not appear to be critical to the encoding.
            args.Request.SetActualStartPosition(TimeSpan.FromTicks(DateTime.Now.Ticks));
        }

        private void OnSampleRequested(MediaStreamSource sender, MediaStreamSourceSampleRequestedEventArgs args)
        {
            // This gets called whenever the encoder wants to encode another sample
            // It gets called for video streams and also audio streams, but not metadata streams
            // There is no other obvious way of adding metadata to a stream, so this is where we are stuck for now

            // Normally you would just use
            // args.Request.Sample = MediaStreamSample.CreateFromDirect3D11Surface()
            // to add the new video sample to the stream
            // But as explained that is not enough to get the other data into the stream
        }

        #endregion
    }
}