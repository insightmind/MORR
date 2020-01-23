using System;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;
using MORR.Core.Data.Capture.Video.WinAPI.Utility;
using MORR.Core.Data.Sample.Video;
using MORR.Shared.Events.Queue;
using MORR.Shared.Events.Queue.Strategy.SingleConsumer;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Device = SharpDX.Direct3D11.Device;

namespace MORR.Core.Data.Capture.Video.WinAPI
{
    [Export(typeof(VideoSampleProducer)), Export(typeof(IReadOnlyEventQueue<VideoSample>))]
    public class VideoSampleProducer : BoundedSingleConsumerEventQueue<VideoSample>
    {
        /// <summary>
        ///     Starts a video capture from the provided capture item.
        /// </summary>
        /// <param name="item">The <see cref="GraphicsCaptureItem" /> to start the video capture from.</param>
        public void StartCapture(GraphicsCaptureItem item)
        {
            InitializeCaptureItem(item);
            InitializeFramePool();
            InitializeSession();
            InitializeBlankTexture();

            Task.Run(EnqueueFrames);
        }

        /// <summary>
        ///     Stops a video capture.
        /// </summary>
        public void StopCapture()
        {
            closedEvent.Set();
            canCleanupNonPersistentResourcesEvent.WaitOne();
            CleanupSessionResources();
        }

        private void EnqueueFrames()
        {
            var currentSample = GetNextFrame();

            while (currentSample != null)
            {
                Enqueue(currentSample);
                currentSample = GetNextFrame();
            }
        }

        private VideoSample? GetNextFrame()
        {
            currentFrame?.Dispose();
            frameEvent.Reset();

            var signaledEvent = events[WaitHandle.WaitAny(events)];
            if (signaledEvent == closedEvent)
            {
                // Safe to cleanup non-persistent resources as this method will return and the code below no longer references them
                canCleanupNonPersistentResourcesEvent.Set();
                return null;
            }

            using (new MultithreadLock(multithread))
            {
                if (currentFrame == null)
                {
                    return null;
                }

                // Copy the captured frame from the framepool to a useable texture
                using var sourceTexture = Direct3D11Helper.CreateSharpDXTexture2D(currentFrame.Surface);
                if (sourceTexture == null)
                {
                    return null;
                }

                var description = sourceTexture.Description;
                description.Usage = ResourceUsage.Default;
                description.BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget;
                description.CpuAccessFlags = CpuAccessFlags.None;
                description.OptionFlags = ResourceOptionFlags.None;

                using var copyTexture = new Texture2D(sharpDXDevice, description);
                var width = Math.Clamp(currentFrame.ContentSize.Width, 0, currentFrame.Surface.Description.Width);
                var height = Math.Clamp(currentFrame.ContentSize.Height, 0, currentFrame.Surface.Description.Height);

                var region = new ResourceRegion(0, 0, 0, width, height, 1);

                sharpDXDevice.ImmediateContext.CopyResource(blankTexture, copyTexture);
                sharpDXDevice.ImmediateContext.CopySubresourceRegion(sourceTexture, 0, region, copyTexture, 0);

                var surface = Direct3D11Helper.CreateDirect3DSurfaceFromSharpDXTexture(copyTexture);
                if (surface == null)
                {
                    return null;
                }

                return new VideoSample
                {
                    Surface = surface
                };
            }
        }

        private class MultithreadLock : IDisposable
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

        #region Fields

        private Texture2D blankTexture;
        private ManualResetEvent canCleanupNonPersistentResourcesEvent;
        private ManualResetEvent closedEvent;
        private Direct3D11CaptureFrame currentFrame;

        private IDirect3DDevice device;

        private WaitHandle[] events;
        private ManualResetEvent frameEvent;
        private Direct3D11CaptureFramePool framePool;

        private GraphicsCaptureItem item;
        private Multithread multithread;
        private GraphicsCaptureSession session;
        private Device sharpDXDevice;

        #endregion

        #region Initialization

        private void InitializeDevices()
        {
            var createdDevice = Direct3D11Helper.CreateDevice();
            device = createdDevice ?? throw new Exception("Failed to create Direct3D device.");

            var createdSharpDXDevice = Direct3D11Helper.CreateSharpDXDevice(device);
            sharpDXDevice = createdSharpDXDevice ?? throw new Exception("Failed to create SharpDX device.");

            multithread = sharpDXDevice.QueryInterface<Multithread>();
            multithread.SetMultithreadProtected(true);
        }

        private void InitializeEvents()
        {
            frameEvent = new ManualResetEvent(false);
            closedEvent = new ManualResetEvent(false);
            events = new WaitHandle[] { closedEvent, frameEvent };
            canCleanupNonPersistentResourcesEvent = new ManualResetEvent(false);
        }

        private void InitializeFramePool()
        {
            framePool = Direct3D11CaptureFramePool.CreateFreeThreaded(device, DirectXPixelFormat.B8G8R8A8UIntNormalized,
                                                                      1, item.Size);
            framePool.FrameArrived += OnFrameArrived;
        }

        private void InitializeSession()
        {
            session = framePool.CreateCaptureSession(item);
            session.StartCapture();
        }

        private void InitializeCaptureItem(GraphicsCaptureItem item)
        {
            this.item = item;
            this.item.Closed += OnClosed;
        }

        public VideoSampleProducer() : base(16)
        {
            InitializeDevices();
            InitializeEvents();
        }

        private void InitializeBlankTexture()
        {
            var description = new Texture2DDescription
            {
                Width = item.Size.Width,
                Height = item.Size.Height,
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
            blankTexture = new Texture2D(sharpDXDevice, description);

            using var renderTargetView = new RenderTargetView(sharpDXDevice, blankTexture);
            sharpDXDevice.ImmediateContext.ClearRenderTargetView(renderTargetView, new RawColor4(0, 0, 0, 1));
        }

        #endregion

        #region Event handlers

        private void OnFrameArrived(Direct3D11CaptureFramePool sender, object args)
        {
            currentFrame = sender.TryGetNextFrame();
            frameEvent.Set();
        }

        private void OnClosed(GraphicsCaptureItem sender, object args)
        {
            StopCapture();
        }

        #endregion

        #region Cleanup

        private void CleanupSessionResources()
        {
            if (framePool != null)
            {
                framePool.FrameArrived -= OnFrameArrived;
                framePool.Dispose();
            }

            framePool = null;

            session?.Dispose();

            if (item != null)
            {
                item.Closed -= OnClosed;
            }

            item = null;
            blankTexture?.Dispose();
            blankTexture = null;
            currentFrame?.Dispose();
            currentFrame = null;
        }

        private void CleanupPersistentResources()
        {
            device?.Dispose();
            device = null;
            sharpDXDevice?.Dispose();
            sharpDXDevice = null;
        }

        public void Dispose()
        {
            StopCapture();
            CleanupPersistentResources();
        }

        #endregion
    }
}