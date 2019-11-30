using MORR.Core.Data.Sample.Audio;

namespace MORR.Core.Data.Transcoding.Audio
{
    /// <summary>
    ///     Handles the <see cref="IDecoder.AudioSampleDecoded" /> event
    /// </summary>
    /// <param name="sample">The <see cref="AudioSample" /> that was decoded</param>
    public delegate void AudioSampleDecodedEventHandler(AudioSample sample);
}