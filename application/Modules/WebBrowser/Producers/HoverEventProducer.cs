using System;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for HoverEvent
    /// </summary>
    [Export(typeof(HoverEventProducer))]
    [Export(typeof(EventQueue<HoverEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(IWebBrowserEventProducer))]
    public class HoverEventProducer : EventQueue<HoverEvent>, IWebBrowserEventProducer
    {
        public HoverEventProducer() : base(new BoundedMultiConsumerChannelStrategy<HoverEvent>(16, null))
        {

        }

        public void Notify(WebBrowserEvent @event)
        {
            if (@event is HoverEvent hoverEvent)
                Enqueue(hoverEvent);
        }

        public Type HandledEventType => typeof(HoverEvent);
    }
}