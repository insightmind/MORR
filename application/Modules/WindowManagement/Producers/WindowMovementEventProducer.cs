using System.ComponentModel.Composition;
using MORR.Modules.WindowManagement.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowMovementEvent
    /// </summary>
    [Export(typeof(WindowMovementEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<WindowMovementEvent>))]
    public class WindowMovementEventProducer : DefaultEventQueue<WindowMovementEvent>
    {
        // TODO: Implement this
    }
}