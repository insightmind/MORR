using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for NavigationEvent
    /// </summary>
    [Export(typeof(NavigationEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<NavigationEvent>))]
    [Export(typeof(WebBrowserEventProducer<NavigationEvent>))]
    [Export(typeof(IWebBrowserEventObserver))]
    [Export(typeof(IReadOnlyEventQueue<Event>))]
    [Export(typeof(ISupportDeserializationEventQueue<NavigationEvent>))]
    public class NavigationEventProducer : WebBrowserEventProducer<NavigationEvent> { }
}