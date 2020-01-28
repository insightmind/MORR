using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for HoverEvent
    /// </summary>
    [Export(typeof(HoverEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<HoverEvent>))]
    [Export(typeof(WebBrowserEventProducer<HoverEvent>))]
    [Export(typeof(IWebBrowserEventObserver))]
    [Export(typeof(IReadOnlyEventQueue<Event>))]
    [Export(typeof(ISupportDeserializationEventQueue<HoverEvent>))]
    public class HoverEventProducer : WebBrowserEventProducer<HoverEvent> { }
}