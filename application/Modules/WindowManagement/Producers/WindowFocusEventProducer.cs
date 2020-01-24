using System.ComponentModel.Composition;
using MORR.Modules.WindowManagement.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowFocusEvent
    /// </summary>
    [Export(typeof(WindowFocusEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<WindowFocusEvent>))]
    public class WindowFocusEventProducer : DefaultEventQueue<WindowFocusEvent>
    {
        // TODO: Implement this
    }
}