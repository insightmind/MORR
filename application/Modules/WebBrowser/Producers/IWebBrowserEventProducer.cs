using System;

namespace MORR.Modules.WebBrowser.Producers
{
    internal interface IWebBrowserEventProducer : IWebBrowserEventObserver
    {
        /// <summary>
        ///     The BrowserEvent type to be handled by this producer.
        /// </summary>
        Type HandledEventType { get; }
    }
}
