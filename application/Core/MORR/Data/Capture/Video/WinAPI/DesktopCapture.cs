using System;
using System.Composition;
using System.Linq;
using Windows.Graphics.Capture;
using MORR.Core.Data.Capture.Video.WinAPI.Utility;
using MORR.Shared.Modules;

namespace MORR.Core.Data.Capture.Video.WinAPI
{
    [Export(typeof(IModule))]
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
            if (GraphicsCaptureHelper.CanCreateItemWithoutPicker || Configuration.PromptUserForMonitorSelection)
            {
                return new GraphicsCapturePicker().PickSingleItemAsync().GetResults();
            }

            var monitors = MonitorEnumerationHelper.GetMonitors();
            var monitor = monitors.ElementAtOrDefault(Configuration.MonitorIndex.Value);

            return monitor != null ? GraphicsCaptureHelper.CreateItemForMonitor(monitor.Hmon) : null;
        }

        private void StartCapture()
        {
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