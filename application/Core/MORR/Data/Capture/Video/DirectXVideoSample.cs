using Windows.Graphics.DirectX.Direct3D11;

namespace MORR.Core.Data.Capture.Video
{
    /// <summary>
    ///     A single video capture sample in DirectX format.
    /// </summary>
    public class DirectXVideoSample : VideoSample
    {
        /// <summary>
        ///     The surface containing the data for this sample.
        /// </summary>
        public IDirect3DSurface Surface { get; set; } = null!;
    }
}