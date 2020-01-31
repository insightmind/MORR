using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for FileDownloadEvent
    /// </summary>
    [Export(typeof(WebBrowserEventProducer<FileDownloadEvent>))]
    [Export(typeof(IWebBrowserEventObserver))]
    public class FileDownloadEventProducer : WebBrowserEventProducer<FileDownloadEvent> { }
}