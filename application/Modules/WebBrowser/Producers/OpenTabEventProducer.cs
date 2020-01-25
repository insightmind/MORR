using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for OpenTabEvent
    /// </summary>
    [Export(typeof(OpenTabEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<OpenTabEvent>))]
    [Export(typeof(WebBrowserEventProducer<OpenTabEvent>))]
    [Export(typeof(IWebBrowserEventObserver))]
    [Export(typeof(IReadOnlyEventQueue<Event>))]
    [Export(typeof(IReadWriteEventQueue<OpenTabEvent>))]
    public class OpenTabEventProducer : WebBrowserEventProducer<OpenTabEvent> { }
}