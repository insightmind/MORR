using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for TextInputEvent
    /// </summary>
    [Export(typeof(WebBrowserEventProducer<TextInputEvent>))]
    [Export(typeof(IWebBrowserEventObserver))]
    public class TextInputEventProducer : WebBrowserEventProducer<TextInputEvent> { }
}