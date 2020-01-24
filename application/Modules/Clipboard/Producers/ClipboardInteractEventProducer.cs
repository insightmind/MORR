using System.ComponentModel.Composition;
using MORR.Modules.Clipboard.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.Clipboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ClipboardInteractEvent
    /// </summary>
    [Export(typeof(ClipboardInteractEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<ClipboardInteractEvent>))]
    [Export(typeof(IReadOnlyEventQueue<Event>))]
    public class ClipboardInteractEventProducer : DefaultEventQueue<ClipboardInteractEvent>
    {
        // TODO: Implement this
    }
}