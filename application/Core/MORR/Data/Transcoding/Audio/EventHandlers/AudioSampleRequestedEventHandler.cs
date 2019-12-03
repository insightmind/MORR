using MORR.Core.Data.Sample.Audio;

namespace MORR.Core.Data.Transcoding.Audio.EventHandlers
{
    /// <summary>
    ///     Handles the <see cref="IEncoder.AudioSampleRequested" /> event
    /// </summary>
    /// <returns>The next <see cref="AudioSample" /> to encode or <see langword="null" /> if there are no more samples</returns>
    public delegate AudioSample? AudioSampleRequestedEventHandler();
}