using System.ComponentModel.Composition;
using MORR.Shared.Utility;

namespace MORR.Core.Data.Transcoding
{
    /// <summary>
    ///     Decodes samples from a file and provides the decoded samples.
    /// </summary>
    [InheritedExport]
    public interface IDecoder
    {
        /// <summary>
        ///     Decodes the file and provides the decoded samples.
        ///     <remarks>This method will not return before the decoding is finished.</remarks>
        /// </summary>
        /// <param name="path">The <see cref="FilePath" /> of the file to decode from.</param>
        void Decode(FilePath path);
    }
}