using MORR.Core.Data.Transcoding.Audio;
using MORR.Core.Data.Transcoding.Metadata;
using MORR.Core.Data.Transcoding.Video;

namespace MORR.Core.Data.Transcoding
{
    public interface IDecoder
    {
        bool IsDecoding { get; set; }
        string DecodingPath { get; set; }

        event AudioSampleDecodedEventHandler AudioSampleDecoded;
        event VideoSampleDecodedEventHandler VideoSampleDecoded;
        event MetadataSampleDecodedEventHandler MetadataSampleDecoded;
    }
}