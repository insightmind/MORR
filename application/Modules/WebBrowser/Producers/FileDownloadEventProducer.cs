using System;
using System.Collections.Generic;
using System.Text;
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
    public class FileDownloadEventProducer : EventQueue<FileDownloadEvent>
    {
        /// <summary>
        ///     Asynchronously gets all file download events as FileDownloadEvent type
        /// </summary>
        /// <returns>A stream of FileDownloadEvent</returns>
        public override IAsyncEnumerable<FileDownloadEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new file download event
        /// </summary>
        /// <param name="event">The file download event to enqueue</param>
        protected override void Enqueue(FileDownloadEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}