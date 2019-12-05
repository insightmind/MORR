using System;
using System.Runtime.InteropServices;
using Windows.Graphics.DirectX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using Device3 = SharpDX.DXGI.Device3;

// TODO This is mostly just copied from the reference project and needs to be cleaned up
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
    ///     Provides utility methods for working with D3D11 objects
    /// </summary>
    public static class Direct3D11Helpers
    {
        private static readonly Guid IInspectable = new Guid("AF86E2E0-B12D-4c6a-9C5A-D7AA65101E90");
        private static readonly Guid ID3D11Resource = new Guid("dc8e63f3-d12b-4952-b47b-5e45026a862d");
        private static readonly Guid IDXGIAdapter3 = new Guid("645967A4-1392-4310-A798-8053CE3E93FD");
        private static readonly Guid ID3D11Device = new Guid("db6f6ddb-ac77-4e88-8253-819df9bbf140");
        private static readonly Guid ID3D11Texture2D = new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c");

        [DllImport("d3d11.dll", EntryPoint = "CreateDirect3D11DeviceFromDXGIDevice", SetLastError = true,
            CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern uint CreateDirect3D11DeviceFromDXGIDevice(IntPtr dxgiDevice, out IntPtr graphicsDevice);

        [DllImport("d3d11.dll", EntryPoint = "CreateDirect3D11SurfaceFromDXGISurface", SetLastError = true,
            CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern uint CreateDirect3D11SurfaceFromDXGISurface(IntPtr dxgiSurface,
                                                                          out IntPtr graphicsSurface);

        public static IDirect3DDevice CreateDevice(bool useWARP = false)
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

        internal static Device CreateSharpDXDevice(IDirect3DDevice device)
        {
            var access = (IDirect3DDxgiInterfaceAccess) device;
            var d3dPointer = access.GetInterface(ID3D11Device);
            var d3dDevice = new Device(d3dPointer);
            return d3dDevice;
        }

        internal static Texture2D CreateSharpDXTexture2D(IDirect3DSurface surface)
        {
            var access = (IDirect3DDxgiInterfaceAccess) surface;
            var d3dPointer = access.GetInterface(ID3D11Texture2D);
            var d3dSurface = new Texture2D(d3dPointer);
            return d3dSurface;
        }
    }
}