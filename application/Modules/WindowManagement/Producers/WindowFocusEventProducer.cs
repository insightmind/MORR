using MORR.Shared.Events.Queue;
using MORR.Modules.WindowManagement.Events;
using System.ComponentModel.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowFocusEvent
    /// </summary>
    [Export(typeof(WindowFocusEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<WindowFocusEvent>))]
    public class WindowFocusEventProducer : BoundedMultiConsumerEventQueue<WindowFocusEvent>
    {
        // TODO: Implement this
    }
}