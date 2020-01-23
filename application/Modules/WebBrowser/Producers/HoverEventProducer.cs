using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for HoverEvent
    /// </summary>
    [Export(typeof(HoverEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<HoverEvent>))]
    [Shared]
    public class HoverEventProducer : BoundedMultiConsumerEventQueue<HoverEvent>
    {
        // TODO: Implement this
    }
}