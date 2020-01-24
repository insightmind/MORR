using System;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for CloseTabEvent
    /// </summary>
    [Export(typeof(CloseTabEventProducer))]
    [Export(typeof(EventQueue<CloseTabEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(IWebBrowserEventProducer))]
    public class CloseTabEventProducer : EventQueue<CloseTabEvent>, IWebBrowserEventProducer
    {
        public CloseTabEventProducer() : base(new BoundedMultiConsumerChannelStrategy<CloseTabEvent>(64, null))
        {

        }
        public void Notify(WebBrowserEvent @event)
        {
            if (@event is CloseTabEvent closeTabEvent)
                Enqueue(closeTabEvent);
        }

        public Type HandledEventType => typeof(CloseTabEvent);
    }
}