using Windows.Graphics.DirectX.Direct3D11;
using MORR.Shared.Events;

namespace MORR.Core.Data.Sample.Video
{
    /// <summary>
    ///     A single video capture sample
    /// </summary>
    public class VideoSample : Event
    {
        /// <summary>
        ///     The surface containing the data for this sample.
        /// </summary>
        public IDirect3DSurface Surface { get; set; }
    }
}