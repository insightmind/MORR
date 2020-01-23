using MORR.Shared.Events.Queue;
using MORR.Modules.Mouse.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseClickEvent
    /// </summary>
    [Export(typeof(MouseClickEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<MouseClickEvent>))]
    [Shared]
    public class MouseClickEventProducer : BoundedMultiConsumerEventQueue<MouseClickEvent>
    {
        // TODO: Implement this
    }
}