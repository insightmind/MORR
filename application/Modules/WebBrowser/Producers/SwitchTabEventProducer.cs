using System;
using System.Collections.Generic;
using System.Text;
using System.Composition;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    [Export(typeof(SwitchTabEventProducer))]
    [Export(typeof(EventQueue<SwitchTabEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class SwitchTabEventProducer : EventQueue<SwitchTabEvent>
    {
        public override IAsyncEnumerable<SwitchTabEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        protected override void Enqueue(SwitchTabEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
