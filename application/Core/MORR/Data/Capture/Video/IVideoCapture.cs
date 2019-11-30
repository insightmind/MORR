using MORR.Core.Data.Sample.Video;

namespace MORR.Core.Data.Capture.Video
{
    /// <summary>
    ///     Captures video output and provides it on a per-sample basis
    /// </summary>
    public interface IVideoCapture
    {
        /// <summary>
        ///     Gets the next <see cref="VideoSample" /> from the capture
        /// </summary>
        /// <returns>The next <see cref="VideoSample" /> from the capture</returns>
        VideoSample NextSample();
    }
}