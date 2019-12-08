using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
namespace MORR.Modules.WebBrowser.Producers
{
    [Export(typeof(FileDownloadEventProducer))]
    [Export(typeof(EventQueue<FileDownloadEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class FileDownloadEventProducer : EventQueue<FileDownloadEvent>
    { 
        public override IAsyncEnumerable<FileDownloadEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        protected override void Enqueue(FileDownloadEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}