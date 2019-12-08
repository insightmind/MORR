using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
namespace MORR.Modules.WebBrowser.Producers
{
    [Export(typeof(TextSelectionEventProducer))]
    [Export(typeof(EventQueue<TextSelectionEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class TextSelectionEventProducer : EventQueue<TextSelectionEvent>
    {
        public override IAsyncEnumerable<TextSelectionEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        protected override void Enqueue(TextSelectionEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}