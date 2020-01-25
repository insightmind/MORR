using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for FileDownloadEvent
    /// </summary>
    [Export(typeof(FileDownloadEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<FileDownloadEvent>))]
    public class FileDownloadEventProducer : DefaultEventQueue<FileDownloadEvent>
    {
        // TODO: Implement this
    }
}