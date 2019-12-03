using MORR.Core.Data.Sample.Video;

namespace MORR.Core.Data.Transcoding.Video.EventHandlers
{
    /// <summary>
    ///     Handles the <see cref="IDecoder.VideoSampleDecoded" /> event
    /// </summary>
    /// <param name="sample">The <see cref="VideoSample" /> that was decoded</param>
    public delegate void VideoSampleDecodedEventHandler(VideoSample sample);
}