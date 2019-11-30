using MORR.Core.Data.Sample.Metadata;

namespace MORR.Core.Data.Transcoding.Metadata
{
    /// <summary>
    ///     Handles the <see cref="IDecoder.MetadataSampleDecoded" /> event
    /// </summary>
    /// <param name="sample">The <see cref="MetadataSample" /> that was decoded</param>
    public delegate void MetadataSampleDecodedEventHandler(MetadataSample sample);
}