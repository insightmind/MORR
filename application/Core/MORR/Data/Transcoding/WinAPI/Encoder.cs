using MORR.Core.Data.Transcoding.Audio.EventHandlers;
using MORR.Core.Data.Transcoding.Metadata.EventHandlers;
using MORR.Core.Data.Transcoding.Video.EventHandlers;

namespace MORR.Core.Data.Transcoding.WinAPI
{
    /// <summary>
    ///     Encodes events into an MPEG-Stream using the Windows API
    /// </summary>
    public class Encoder : IEncoder
    {
        public bool IsEncoding { get; set; }
        public string EncodingPath { get; set; }
        public event AudioSampleRequestedEventHandler AudioSampleRequested;
        public event VideoSampleRequestedEventHandler VideoSampleRequested;
        public event MetadataSampleRequestedEventHandler MetadataSampleRequested;
    }
}