using MORR.Core.Data.Transcoding.Audio.EventHandlers;
using MORR.Core.Data.Transcoding.Metadata.EventHandlers;
using MORR.Core.Data.Transcoding.Video.EventHandlers;
using MORR.Shared.Modules;

namespace MORR.Core.Data.Transcoding
{
    /// <summary>
    ///     Encodes provided samples to a path
    /// </summary>
    public interface IEncoder : IReceivingModule
    {
        /// <summary>
        ///     <see langword="true" /> if the encoder is encoding, <see langword="false" /> otherwise
        /// </summary>
        bool IsEncoding { get; set; }

        /// <summary>
        ///     The path the encoder is encoding to
        /// </summary>
        string EncodingPath { get; set; }

        /// <summary>
        ///     Event raised when the next <see cref="Sample.Audio.AudioSample" /> may be added
        /// </summary>
        event AudioSampleRequestedEventHandler AudioSampleRequested;

        /// <summary>
        ///     Event raised when the next <see cref="Sample.Video.VideoSample" /> may be added
        /// </summary>
        event VideoSampleRequestedEventHandler VideoSampleRequested;

        /// <summary>
        ///     Event raised when the next <see cref="Sample.Metadata.MetadataSample" /> may be added
        /// </summary>
        event MetadataSampleRequestedEventHandler MetadataSampleRequested;
    }
}