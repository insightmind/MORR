using MORR.Shared.Utility;

namespace MORR.Core.Data.Transcoding
{
    /// <summary>
    ///     Decodes samples from a file and provides the decoded samples.
    /// </summary>
    public interface IDecoder
    {
        /// <summary>
        ///     Decodes the recording and provides the decoded samples.
        /// </summary>
        /// <param name="path">The <see cref="DirectoryPath" /> of the file to decode from.</param>
        void Decode(DirectoryPath path);
    }
}