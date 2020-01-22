using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for FileDownloadEvent
    /// </summary>
    [Export(typeof(FileDownloadEventProducer))]
    [Export(typeof(EventQueue<FileDownloadEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class FileDownloadEventProducer : BoundedMultiConsumerEventQueue<FileDownloadEvent>
    {
        // TODO: Implement this
    }
}