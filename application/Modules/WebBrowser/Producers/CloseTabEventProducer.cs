using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for CloseTabEvent
    /// </summary>
    [Export(typeof(WebBrowserEventProducer<CloseTabEvent>))]
    [Export(typeof(IWebBrowserEventObserver))]
    public class CloseTabEventProducer : WebBrowserEventProducer<CloseTabEvent> { }
}