using System;
using System.Threading;
using Windows.Graphics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;
using MORR.Core.Data.Capture.Video.WinAPI.Utility;
using MORR.Core.Data.Sample.Video;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Device = SharpDX.Direct3D11.Device;

// TODO This is copied from the reference project and has not been cleaned up at all
namespace MORR.Core.Data.Capture.Video.WinAPI
{
    // Don't model this in UML
    // TODO Make this compatible with VideoSample
    public sealed class SurfaceWithInfo : IDisposable
    {
        public IDirect3DSurface Surface { get; internal set; }
        public TimeSpan SystemRelativeTime { get; internal set; }

        public void Dispose()
        {
            Surface?.Dispose();
            Surface = null;
        }
    }

    // TODO Probably delete this
    internal class MultithreadLock : IDisposable
    {
        private Multithread multithread;

        public MultithreadLock(Multithread multithread)
        {
            this.multithread = multithread;
            this.multithread?.Enter();
        }

        public void Dispose()
        {
            multithread?.Leave();
            multithread = null;
        }
    }

    /// <summary>
    ///     Captures the desktop using the Windows API
    /// </summary>
    public sealed class DesktopCapture : IVideoCapture, IDisposable
    {
        private readonly ManualResetEvent closedEvent;

        private readonly ManualResetEvent[] events;
        private readonly ManualResetEvent frameEvent;
        private readonly Multithread multithread;
        private Texture2D blankTexture;
        private Direct3D11CaptureFrame currentFrame;
        private Device d3dDevice;

        private IDirect3DDevice device;
        private Direct3D11CaptureFramePool framePool;

        private GraphicsCaptureItem item;
        private GraphicsCaptureSession session;

        public DesktopCapture(IDirect3DDevice device,
                              GraphicsCaptureItem item,
                              SizeInt32 size)
        {
            this.device = device;
            d3dDevice = Direct3D11Helpers.CreateSharpDXDevice(device);
            multithread = d3dDevice.QueryInterface<Multithread>();
            multithread.SetMultithreadProtected(true);
            this.item = item;
            frameEvent = new ManualResetEvent(false);
            closedEvent = new ManualResetEvent(false);
            events = new[] { closedEvent, frameEvent };

            InitializeBlankTexture(size);
            InitializeCapture(size);
        }

        // Probably should not be modeled
        public void Dispose()
        {
            Stop();
            Cleanup();
        }


        public VideoSample NextSample()
        {
            // TODO Implement this
            throw new NotImplementedException();
        }

        private void InitializeCapture(SizeInt32 size)
        {
            item.Closed += OnClosed;
            framePool = Direct3D11CaptureFramePool.CreateFreeThreaded(device, DirectXPixelFormat.B8G8R8A8UIntNormalized,
                                                                      1, size);
            framePool.FrameArrived += OnFrameArrived;
            session = framePool.CreateCaptureSession(item);
            session.StartCapture();
        }

        private void InitializeBlankTexture(SizeInt32 size)
        {
            var description = new Texture2DDescription
            {
                Width = size.Width,
                Height = size.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription
                {
                    Count = 1,
                    Quality = 0
                },
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };
            blankTexture = new Texture2D(d3dDevice, description);

            using var renderTargetView = new RenderTargetView(d3dDevice, blankTexture);
            d3dDevice.ImmediateContext.ClearRenderTargetView(renderTargetView, new RawColor4(0, 0, 0, 1));
        }

        private void SetResult(Direct3D11CaptureFrame frame)
        {
            currentFrame = frame;
            frameEvent.Set();
        }

        private void Stop()
        {
            closedEvent.Set();
        }

        private void OnFrameArrived(Direct3D11CaptureFramePool sender, object args)
        {
            SetResult(sender.TryGetNextFrame());
        }

        private void OnClosed(GraphicsCaptureItem sender, object args)
        {
            Stop();
        }

        private void Cleanup()
        {
            framePool?.Dispose();
            session?.Dispose();
            if (item != null)
            {
                item.Closed -= OnClosed;
            }

            item = null;
            device = null;
            d3dDevice = null;
            blankTexture?.Dispose();
            blankTexture = null;
            currentFrame?.Dispose();
        }

        // Note: This would probably be moved to the NextSample-Method and should not be modelled in UML
        public SurfaceWithInfo WaitForNewFrame()
        {
            // Let's get a fresh one.
            currentFrame?.Dispose();
            frameEvent.Reset();

            var signaledEvent = events[WaitHandle.WaitAny(events)];
            if (signaledEvent == closedEvent)
            {
                //Cleanup();
                return null;
            }

            var result = new SurfaceWithInfo { SystemRelativeTime = currentFrame.SystemRelativeTime };

            using var sourceTexture = Direct3D11Helpers.CreateSharpDXTexture2D(currentFrame.Surface);
            var description = sourceTexture.Description;
            description.Usage = ResourceUsage.Default;
            description.BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget;
            description.CpuAccessFlags = CpuAccessFlags.None;
            description.OptionFlags = ResourceOptionFlags.None;

            using var copyTexture = new Texture2D(d3dDevice, description);
            var width = Math.Clamp(currentFrame.ContentSize.Width, 0, currentFrame.Surface.Description.Width);
            var height = Math.Clamp(currentFrame.ContentSize.Height, 0, currentFrame.Surface.Description.Height);

            var region = new ResourceRegion(0, 0, 0, width, height, 1);

            d3dDevice.ImmediateContext.CopyResource(blankTexture, copyTexture);
            d3dDevice.ImmediateContext.CopySubresourceRegion(sourceTexture, 0, region, copyTexture, 0);
            result.Surface = Direct3D11Helpers.CreateDirect3DSurfaceFromSharpDXTexture(copyTexture);

            return result;
        }
    }
}