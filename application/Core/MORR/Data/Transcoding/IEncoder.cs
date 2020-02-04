using System.Threading;
using MORR.Shared.Utility;

namespace MORR.Core.Data.Transcoding
{
    /// <summary>
    ///     Encodes provided samples to a file.
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        ///     An event raised when encoding finishes.
        /// </summary>
        ManualResetEvent EncodeFinished { get; }

        /// <summary>
        ///     Encodes the provided samples to a file.
        /// </summary>
        /// <param name="recordingDirectoryPath">The <see cref="DirectoryPath" /> to contain the recording.</param>
        void Encode(DirectoryPath recordingDirectoryPath);
    }
}