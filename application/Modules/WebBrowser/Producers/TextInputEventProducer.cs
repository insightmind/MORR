using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
namespace MORR.Modules.WebBrowser.Producers
{
    [Export(typeof(TextInputEventProducer))]
    [Export(typeof(EventQueue<TextInputEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class TextInputEventProducer : EventQueue<TextInputEvent>
    {
        public override IAsyncEnumerable<TextInputEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        protected override void Enqueue(TextInputEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}