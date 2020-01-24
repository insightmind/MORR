using System.ComponentModel.Composition;
using MORR.Modules.WindowManagement.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowResizingEvent
    /// </summary>
    [Export(typeof(WindowResizingEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<WindowResizingEvent>))]
    public class WindowResizingEventProducer : DefaultEventQueue<WindowResizingEvent>
    {
        // TODO: Implement this
    }
}