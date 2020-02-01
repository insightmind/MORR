using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for OpenTabEvent
    /// </summary>
    [Export(typeof(WebBrowserEventProducer<OpenTabEvent>))]
    [Export(typeof(IWebBrowserEventObserver))]
    public class OpenTabEventProducer : WebBrowserEventProducer<OpenTabEvent> { }
}