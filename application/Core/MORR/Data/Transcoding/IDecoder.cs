using MORR.Core.Data.Transcoding.Audio;
using MORR.Core.Data.Transcoding.Metadata;
using MORR.Core.Data.Transcoding.Video;

namespace MORR.Core.Data.Transcoding
{
    /// <summary>
    ///     Decodes samples from a path and provides the decoded samples
    /// </summary>
    public interface IDecoder
    {
        /// <summary>
        ///     <see langword="true" /> when the decoder is decoding, <see langword="false" /> otherwise
        /// </summary>
        bool IsDecoding { get; set; }

        /// <summary>
        ///     The path the decoder is decoding to
        /// </summary>
        string DecodingPath { get; set; }

        /// <summary>
        ///     Event raised when a <see cref="Sample.Audio.AudioSample" /> gets decoded
        /// </summary>
        event AudioSampleDecodedEventHandler AudioSampleDecoded;

        /// <summary>
        ///     Event raised when a <see cref="Sample.Video.VideoSample" /> get decoded
        /// </summary>
        event VideoSampleDecodedEventHandler VideoSampleDecoded;

        /// <summary>
        ///     Event raised when a <see cref="Sample.Metadata.MetadataSample" /> gets decoded
        /// </summary>
        event MetadataSampleDecodedEventHandler MetadataSampleDecoded;
    }
}