using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
namespace MORR.Modules.WebBrowser.Producers
{
    [Export(typeof(NavigationEventProducer))]
    [Export(typeof(EventQueue<NavigationEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class NavigationEventProducer : EventQueue<NavigationEvent>
    {
        public override IAsyncEnumerable<NavigationEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        protected override void Enqueue(NavigationEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}