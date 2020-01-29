using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ButtonClickEvent
    /// </summary>
    [Export(typeof(WebBrowserEventProducer<ButtonClickEvent>))]
    [Export(typeof(IWebBrowserEventObserver))]
    public class ButtonClickEventProducer : WebBrowserEventProducer<ButtonClickEvent> { }
}