using System;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for OpenTabEvent
    /// </summary>
    [Export(typeof (OpenTabEventProducer))]
    [Export(typeof(EventQueue<OpenTabEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(IWebBrowserEventProducer))]
    public class OpenTabEventProducer : EventQueue<OpenTabEvent>, IWebBrowserEventProducer
    {
        public OpenTabEventProducer() : base(new BoundedMultiConsumerChannelStrategy<OpenTabEvent>(64, null))
        {

        }

        public void Notify(WebBrowserEvent @event)
        {
            if (@event is OpenTabEvent openTabEvent)
                Enqueue(openTabEvent);
        }

        public Type HandledEventType => typeof(OpenTabEvent);
    }
}
