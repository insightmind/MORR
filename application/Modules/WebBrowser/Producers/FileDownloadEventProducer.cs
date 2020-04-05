using System.Text.Json;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for FileDownloadEvent
    /// </summary>
    public class FileDownloadEventProducer : WebBrowserEventProducer<FileDownloadEvent>
    {
        public override EventLabel HandledEventLabel => EventLabel.DOWNLOAD;
    }
}