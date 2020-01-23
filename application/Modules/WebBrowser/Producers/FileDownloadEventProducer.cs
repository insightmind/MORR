﻿using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for FileDownloadEvent
    /// </summary>
    [Export(typeof(FileDownloadEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<FileDownloadEvent>))]
    [Shared]
    public class FileDownloadEventProducer : BoundedMultiConsumerEventQueue<FileDownloadEvent>
    {
        // TODO: Implement this
    }
}