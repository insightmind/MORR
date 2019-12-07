using MORR.Core.Data.Transcoding.Audio.EventHandlers;
using MORR.Core.Data.Transcoding.Metadata.EventHandlers;
using MORR.Core.Data.Transcoding.Video.EventHandlers;

namespace MORR.Core.Data.Transcoding.WinAPI
{
    /// <summary>
    ///     Decodes an MPEG-Stream using the Windows API
    /// </summary>
    public class Decoder : IDecoder
    {
        public bool IsDecoding { get; set; }
        public string DecodingPath { get; set; }
        public event AudioSampleDecodedEventHandler AudioSampleDecoded;
        public event VideoSampleDecodedEventHandler VideoSampleDecoded;
        public event MetadataSampleDecodedEventHandler MetadataSampleDecoded;
    }
}