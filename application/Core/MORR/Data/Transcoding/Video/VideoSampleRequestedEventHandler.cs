using MORR.Core.Data.Sample.Video;

namespace MORR.Core.Data.Transcoding.Video
{
    /// <summary>
    ///     Handles the <see cref="IEncoder.VideoSampleRequested" /> event
    /// </summary>
    /// <returns>The next <see cref="VideoSample" /> to encode or <see langword="null" /> if there are no more samples</returns>
    public delegate VideoSample? VideoSampleRequestedEventHandler();
}