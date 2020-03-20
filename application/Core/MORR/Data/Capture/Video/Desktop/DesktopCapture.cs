using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Windows.Graphics.Capture;
using MORR.Core.Data.Capture.Video.Desktop.Utility;
using MORR.Core.Data.Capture.Video.Exceptions;
using MORR.Shared.Modules;

namespace MORR.Core.Data.Capture.Video.Desktop
{
    public class DesktopCapture : IModule
    {
        private bool isActive;

        [Import]
        private VideoSampleProducer VideoSampleProducer { get; set; } = null!;

        [Import]
        private DesktopCaptureConfiguration Configuration { get; set; } = null!;

        public bool IsActive
        {
            get => isActive;
            set => Shared.Utility.Utility.SetAndDispatch(ref isActive, value, StartCapture, StopCapture);
        }

        public Guid Identifier { get; } = new Guid("9F1D496E-9939-4BE1-9117-6DE21A3D1CFE");

        public void Initialize(bool isEnabled)
        {
            if (isEnabled)
            {
                VideoSampleProducer.Open();
            }
            else
            {
                VideoSampleProducer.Close();
            }
        }

        private bool TryGetGraphicsCaptureItem([NotNullWhen(true)] out GraphicsCaptureItem? item)
        {
            // On versions prior to 1903, the user will always have to manually select the window/monitor to capture
            if (!GraphicsCaptureHelper.CanCreateItemWithoutPicker || Configuration.PromptUserForMonitorSelection)
            {
                var picker = new GraphicsCapturePicker();
                using var handleWrapper = DesktopCaptureNativeMethods.GetAssociatedWindow();

                picker.SetWindow(handleWrapper.Handle);
                item = picker.PickSingleItemAsync().GetAwaiter().GetResult();
                return item != null;
            }

            var monitors = MonitorEnumerationHelper.GetMonitors();
            var monitor = monitors.ElementAtOrDefault(Configuration.MonitorIndex.Value);

            item = monitor != null ? GraphicsCaptureHelper.CreateItemForMonitor(monitor.Hmon) : null;
            return item != null;
        }

        private void StartCapture()
        {
            if (!GraphicsCaptureSession.IsSupported())
            {
                throw new VideoCaptureException("Screen capture is not supported on this device.");
            }

            if (TryGetGraphicsCaptureItem(out var item))
            {
                VideoSampleProducer.StartCapture(item);
            }
            else
            {
                throw new VideoCaptureException("Failed to acquire capture item.");
            }
        }

        private void StopCapture()
        {
            VideoSampleProducer.StopCapture();
        }
    }
}