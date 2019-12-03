using MORR.Core.Data.Sample.Metadata;

namespace MORR.Core.Data.Transcoding.Metadata.EventHandlers
{
    /// <summary>
    ///     Handles the <see cref="IEncoder.MetadataSampleRequested" /> event
    /// </summary>
    /// <returns>The next <see cref="MetadataSample" /> to encoded or <see langword="null" /> if there are no more samples</returns>
    public delegate MetadataSample? MetadataSampleRequestedEventHandler();
}