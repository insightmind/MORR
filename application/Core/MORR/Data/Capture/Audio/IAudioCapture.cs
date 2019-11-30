using MORR.Core.Data.Sample.Audio;

namespace MORR.Core.Data.Capture.Audio
{
    public interface IAudioCapture
    {
        AudioSample NextSample();
    }
}