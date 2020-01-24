using MORR.Shared.Events.Queue;
using MORR.Modules.Mouse.Events;
using System.ComponentModel.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseScrollEvent
    /// </summary>
    [Export(typeof(MouseScrollEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<MouseScrollEvent>))]
    public class MouseScrollEventProducer : BoundedMultiConsumerEventQueue<MouseScrollEvent>
    {
        // TODO: Implement this
    }
}