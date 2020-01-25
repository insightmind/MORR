using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
﻿using System.ComponentModel.Composition;
using MORR.Shared.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for FileDownloadEvent
    /// </summary>
    [Export(typeof(FileDownloadEventProducer))]

    [Export(typeof(IReadOnlyEventQueue<FileDownloadEvent>))]
    [Export(typeof(WebBrowserEventProducer<FileDownloadEvent>))]
    [Export(typeof(IWebBrowserEventObserver))]
    [Export(typeof(IReadOnlyEventQueue<Event>))]
    [Export(typeof(IReadWriteEventQueue<FileDownloadEvent>))]
    public class FileDownloadEventProducer :  WebBrowserEventProducer<FileDownloadEvent>
    {

    }
}