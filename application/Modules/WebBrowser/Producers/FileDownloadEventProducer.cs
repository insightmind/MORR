using System;
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
    [Export(typeof(IWebBrowserEventProducer))]
    public class FileDownloadEventProducer : EventQueue<FileDownloadEvent>, IWebBrowserEventProducer
    {
        public FileDownloadEventProducer() : base(new BoundedMultiConsumerChannelStrategy<FileDownloadEvent>(64, null))
        {

        }

        public void Notify(WebBrowserEvent @event)
        {
            if (@event is FileDownloadEvent fileDownloadEvent)
                Enqueue(fileDownloadEvent);
        }

        public Type HandledEventType => typeof(FileDownloadEvent);
    }
}