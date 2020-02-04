using System.ComponentModel.Composition;
using MORR.Modules.WindowManagement.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowResizingEvent
    /// </summary>
    public class WindowResizingEventProducer : DefaultEventQueue<WindowResizingEvent>
    {
        public void StartCapture() { }

        public void StopCapture() { }
    }
}