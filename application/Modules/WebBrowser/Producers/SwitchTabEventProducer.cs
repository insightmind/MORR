using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for SwitchTabEvent
    /// </summary>
    [Export(typeof(SwitchTabEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<SwitchTabEvent>))]
    [Export(typeof(WebBrowserEventProducer<SwitchTabEvent>))]
    [Export(typeof(IWebBrowserEventObserver))]
    [Export(typeof(IReadOnlyEventQueue<Event>))]
    [Export(typeof(IReadWriteEventQueue<SwitchTabEvent>))]
    public class SwitchTabEventProducer : WebBrowserEventProducer<SwitchTabEvent> { }
}