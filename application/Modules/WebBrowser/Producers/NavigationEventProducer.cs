using System;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for NavigationEvent
    /// </summary>
    [Export(typeof(NavigationEventProducer))]
    [Export(typeof(EventQueue<NavigationEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(IWebBrowserEventProducer))]
    public class NavigationEventProducer : EventQueue<NavigationEvent>, IWebBrowserEventProducer
    {
        public NavigationEventProducer() : base(new BoundedMultiConsumerChannelStrategy<NavigationEvent>(64, null)) { }
        public void Notify(WebBrowserEvent @event)
        {
            if (@event is NavigationEvent navigationEvent)
                Enqueue(navigationEvent);
        }

        public Type HandledEventType => typeof(NavigationEvent);
    }
}