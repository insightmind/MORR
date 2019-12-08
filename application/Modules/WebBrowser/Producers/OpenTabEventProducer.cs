using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
namespace MORR.Modules.WebBrowser.Producers
{
    [Export(typeof (OpenTabEventProducer))]
    [Export(typeof(EventQueue<OpenTabEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class OpenTabEventProducer : EventQueue<OpenTabEvent>
    {
        public override IAsyncEnumerable<OpenTabEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        protected override void Enqueue(OpenTabEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
