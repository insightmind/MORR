using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
namespace MORR.Modules.WebBrowser.Producers
{
    [Export(typeof(HoverEventProducer))]
    [Export(typeof(EventQueue<HoverEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class HoverEventProducer : EventQueue<HoverEvent>
    {
        public override IAsyncEnumerable<HoverEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        protected override void Enqueue(HoverEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}