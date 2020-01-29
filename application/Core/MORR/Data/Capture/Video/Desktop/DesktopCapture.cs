using System;
using System.ComponentModel.Composition;
using System.Linq;
using Windows.Graphics.Capture;
using MORR.Core.Data.Capture.Video.Desktop.Utility;
using MORR.Shared.Modules;
using MORR.Shared.Utility;

namespace MORR.Core.Data.Capture.Video.Desktop
{
    public class DesktopCapture : ICollectingModule
    {
        private bool isActive;

        [Import]
        private VideoSampleProducer VideoSampleProducer { get; set; }

        [Import]
        private DesktopCaptureConfiguration Configuration { get; set; }

        public bool IsActive
        {
            get => isActive;
            set => Shared.Utility.Utility.SetAndDispatch(ref isActive, value, StartCapture, StopCapture);
        }

        public Guid Identifier { get; } = new Guid("9F1D496E-9939-4BE1-9117-6DE21A3D1CFE");

        // TODO This also appears to be empty somewhat often - we could provide an empty implementation in the interface
        public void Initialize() { }

        private GraphicsCaptureItem? GetGraphicsCaptureItem()
        {
            // On versions prior to 1903, the user will always have to manually select the window/monitor to capture
            if (!GraphicsCaptureHelper.CanCreateItemWithoutPicker || Configuration.PromptUserForMonitorSelection)
            {
                var picker = new GraphicsCapturePicker();
                var handle = NativeMethods.GetAssociatedWindow();

                picker.SetWindow(handle);
                return picker.PickSingleItemAsync().GetAwaiter().GetResult();
            }

            var monitors = MonitorEnumerationHelper.GetMonitors();
            var monitor = monitors.ElementAtOrDefault(Configuration.MonitorIndex.Value);

            return monitor != null ? GraphicsCaptureHelper.CreateItemForMonitor(monitor.Hmon) : null;
        }

        private void StartCapture()
        {
            if (!GraphicsCaptureSession.IsSupported())
            {
                throw new Exception("Screen capture is not supported on this device.");
            }

            var item = GetGraphicsCaptureItem();

            if (item != null)
            {
                VideoSampleProducer.StartCapture(item);
            }
        }

        private void StopCapture()
        {
            VideoSampleProducer.StopCapture();
        }
    }
}