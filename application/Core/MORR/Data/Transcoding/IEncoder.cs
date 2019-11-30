using MORR.Core.Data.Transcoding.Audio;
using MORR.Core.Data.Transcoding.Metadata;
using MORR.Core.Data.Transcoding.Video;

namespace MORR.Core.Data.Transcoding
{
    public interface IEncoder
    {
        bool IsEncoding { get; set; }
        string EncodingPath { get; set; }

        event AudioSampleRequestedEventHandler AudioSampleRequested;
        event VideoSampleRequestedEventHandler VideoSampleRequested;
        event MetadataSampleRequestedEventHandler MetadataSampleRequested;
    }
}