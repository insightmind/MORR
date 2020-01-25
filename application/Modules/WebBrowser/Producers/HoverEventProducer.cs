using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events.Queue;
using System.ComponentModel.Composition;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for HoverEvent
    /// </summary>
    [Export(typeof(HoverEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<HoverEvent>))]
    [Export(typeof(WebBrowserEventProducer<HoverEvent>))]
    [Export(typeof(WebBrowserEventProducer<WebBrowserEvent>))]
    public class HoverEventProducer : WebBrowserEventProducer<HoverEvent>
    {

    }
}