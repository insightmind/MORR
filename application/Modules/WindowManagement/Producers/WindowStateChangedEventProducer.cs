using System.ComponentModel.Composition;
using MORR.Modules.WindowManagement.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowStateChangedEvent
    /// </summary>
    public class WindowStateChangedEventProducer : DefaultEventQueue<WindowStateChangedEvent>
    {
        public void StartCapture() { }

        public void StopCapture() { }
    }
}