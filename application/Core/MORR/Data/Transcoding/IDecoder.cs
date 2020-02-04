using System.Threading;
using MORR.Shared.Utility;

namespace MORR.Core.Data.Transcoding
{
    /// <summary>
    ///     Decodes samples from a file and provides the decoded samples.
    /// </summary>
    public interface IDecoder
    {
        /// <summary>
        ///     An event raised when decoding finishes.
        /// </summary>
        ManualResetEvent DecodeFinished { get; }

        /// <summary>
        ///     Decodes the recording and provides the decoded samples.
        /// </summary>
        /// <param name="recordingDirectoryPath">The <see cref="DirectoryPath" /> of the file to decode from.</param>
        void Decode(DirectoryPath recordingDirectoryPath);
    }
}