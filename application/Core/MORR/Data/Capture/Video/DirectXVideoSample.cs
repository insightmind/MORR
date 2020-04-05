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
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public IDirect3DSurface Surface { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    }
}