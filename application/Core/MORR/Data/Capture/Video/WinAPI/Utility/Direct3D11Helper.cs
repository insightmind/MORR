using System;
using System.Runtime.InteropServices;
using Windows.Graphics.DirectX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using Device3 = SharpDX.DXGI.Device3;

namespace MORR.Core.Data.Capture.Video.WinAPI.Utility
{
    [ComImport]
    [Guid("A9B3D012-3DF2-4EE3-B8D1-8695F457D3C1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    internal interface IDirect3DDxgiInterfaceAccess
    {
        IntPtr GetInterface([In] ref Guid iid);
    }

    /// <summary>
    ///     Provides utility methods for using Direct3D and SharpDX objects.
    /// </summary>
    internal static class Direct3D11Helper
    {
        private static readonly Guid ID3D11Device = new Guid("db6f6ddb-ac77-4e88-8253-819df9bbf140");
        private static readonly Guid ID3D11Texture2D = new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c");

        [DllImport("d3d11.dll", EntryPoint = "CreateDirect3D11DeviceFromDXGIDevice", SetLastError = true,
                   CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern uint CreateDirect3D11DeviceFromDXGIDevice(IntPtr dxgiDevice, out IntPtr graphicsDevice);

        [DllImport("d3d11.dll", EntryPoint = "CreateDirect3D11SurfaceFromDXGISurface", SetLastError = true,
                   CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern uint CreateDirect3D11SurfaceFromDXGISurface(IntPtr dxgiSurface,
                                                                          out IntPtr graphicsSurface);

        /// <summary>
        ///     Creates a new Direct3D device.
        /// </summary>
        /// <param name="useWARP">
        ///     Indicates if the device should use WARP emulation (software rendering). <see langword="true" />
        ///     if the device should use WARP, <see langword="false" /> otherwise.
        /// </param>
        /// <returns>The new Direct3D device.</returns>
        internal static IDirect3DDevice CreateDevice(bool useWARP = false)
        {
            using var d3dDevice = new Device(useWARP ? DriverType.Software : DriverType.Hardware,
                                             DeviceCreationFlags.BgraSupport);
            IDirect3DDevice device = null;

            // Acquire the DXGI interface for the Direct3D device.
            using (var dxgiDevice = d3dDevice.QueryInterface<Device3>())
            {
                // Wrap the native device using a WinRT interop object.
                var hr = CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice.NativePointer, out var pUnknown);

                if (hr == 0)
                {
                    device = Marshal.GetObjectForIUnknown(pUnknown) as IDirect3DDevice;
                    Marshal.Release(pUnknown);
                }
            }

            return device;
        }

        /// <summary>
        ///     Creates a Direct3D surface from a SharpDX texture.
        /// </summary>
        /// <param name="texture">The texture to create a Direct3D surface for.</param>
        /// <returns>The created Direct3D surface.</returns>
        internal static IDirect3DSurface CreateDirect3DSurfaceFromSharpDXTexture(Texture2D texture)
        {
            IDirect3DSurface surface = null;

            // Acquire the DXGI interface for the Direct3D surface.
            using (var dxgiSurface = texture.QueryInterface<Surface>())
            {
                // Wrap the native device using a WinRT interop object.
                var hr = CreateDirect3D11SurfaceFromDXGISurface(dxgiSurface.NativePointer, out var pUnknown);

                if (hr == 0)
                {
                    surface = Marshal.GetObjectForIUnknown(pUnknown) as IDirect3DSurface;
                    Marshal.Release(pUnknown);
                }
            }

            return surface;
        }

        /// <summary>
        ///     Creates a SharpDX device from a Direct3D device.
        /// </summary>
        /// <param name="device">The device to create the SharpDX device from.</param>
        /// <returns>The created SharpDX device.</returns>
        internal static Device CreateSharpDXDevice(IDirect3DDevice device)
        {
            var access = (IDirect3DDxgiInterfaceAccess) device;
            var d3dPointer = access.GetInterface(ID3D11Device);
            var d3dDevice = new Device(d3dPointer);
            return d3dDevice;
        }

        /// <summary>
        ///     Creates a SharpDX texture from a Direct3D surface.
        /// </summary>
        /// <param name="surface">The surface to create the SharpDX texture from.</param>
        /// <returns>The created SharpDX texture.</returns>
        internal static Texture2D CreateSharpDXTexture2D(IDirect3DSurface surface)
        {
            var access = (IDirect3DDxgiInterfaceAccess) surface;
            var d3dPointer = access.GetInterface(ID3D11Texture2D);
            var d3dSurface = new Texture2D(d3dPointer);
            return d3dSurface;
        }
    }
}