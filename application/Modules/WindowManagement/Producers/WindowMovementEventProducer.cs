using MORR.Shared.Events.Queue;
using MORR.Modules.WindowManagement.Events;
using System.ComponentModel.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowMovementEvent
    /// </summary>
    [Export(typeof(WindowMovementEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<WindowMovementEvent>))]
    public class WindowMovementEventProducer : BoundedMultiConsumerEventQueue<WindowMovementEvent>
    {
        // TODO: Implement this
    }
}