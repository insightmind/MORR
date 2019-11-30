using MORR.Core.Data.Sample.Audio;

namespace MORR.Core.Data.Capture.Audio
{
    /// <summary>
    ///     Captures audio output and provides it on a per-sample basis
    /// </summary>
    public interface IAudioCapture
    {
        /// <summary>
        ///     Gets the next <see cref="AudioSample" /> from the capture
        /// </summary>
        /// <returns>The next <see cref="AudioSample" /> from the capture</returns>
        AudioSample NextSample();
    }
}