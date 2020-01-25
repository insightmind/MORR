using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for HoverEvent
    /// </summary>
    [Export(typeof(HoverEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<HoverEvent>))]
    public class HoverEventProducer : DefaultEventQueue<HoverEvent>
    {
        // TODO: Implement this
    }
}