using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for SwitchTabEvent
    /// </summary>
    [Export(typeof(WebBrowserEventProducer<SwitchTabEvent>))]
    [Export(typeof(IWebBrowserEventObserver))]
    public class SwitchTabEventProducer : WebBrowserEventProducer<SwitchTabEvent> { }
}