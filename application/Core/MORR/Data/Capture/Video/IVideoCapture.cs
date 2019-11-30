using MORR.Core.Data.Sample.Video;

namespace MORR.Core.Data.Capture.Video
{
    public interface IVideoCapture
    {
        VideoSample NextSample();
    }
}