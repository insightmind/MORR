using MORR.Shared.Events.Queue;
using MORR.Modules.WindowManagement.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowStateChangedEvent
    /// </summary>
    [Export(typeof(WindowStateChangedEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<WindowStateChangedEvent>))]
    [Shared]
    public class WindowStateChangedEventProducer : BoundedMultiConsumerEventQueue<WindowStateChangedEvent>
    {
        // TODO: Implement this
    }
}