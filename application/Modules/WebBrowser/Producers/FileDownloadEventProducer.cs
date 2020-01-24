using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for FileDownloadEvent
    /// </summary>
    [Export(typeof(FileDownloadEventProducer))]
    [Export(typeof(EventQueue<FileDownloadEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(WebBrowserEventProducer<FileDownloadEvent>))]
    [Export(typeof(WebBrowserEventProducer<WebBrowserEvent>))]
    public class FileDownloadEventProducer :  WebBrowserEventProducer<FileDownloadEvent>
    {
    }
}