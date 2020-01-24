using MORR.Shared.Events.Queue;
using MORR.Modules.Mouse.Events;
using System.ComponentModel.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseMoveEvent
    /// </summary>
    [Export(typeof(MouseMoveEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<MouseMoveEvent>))]
    public class MouseMoveEventProducer : BoundedMultiConsumerEventQueue<MouseMoveEvent>
    {
        // TODO: Implement this
    }
}