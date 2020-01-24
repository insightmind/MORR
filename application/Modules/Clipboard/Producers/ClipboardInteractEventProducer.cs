using System.ComponentModel.Composition;
using MORR.Modules.Clipboard.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.Clipboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ClipboardInteractEvent
    /// </summary>
    [Export(typeof(ClipboardInteractEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<ClipboardInteractEvent>))]
    public class ClipboardInteractEventProducer : BoundedMultiConsumerEventQueue<ClipboardInteractEvent>
    {
        // TODO: Implement this
    }
}