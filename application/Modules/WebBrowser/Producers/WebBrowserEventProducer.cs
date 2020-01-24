using System;
using System.Collections.Specialized;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     A generic producer for WebBrowserEvents, which will need to be subscribed to a <see cref="IWebBrowserEventObservible"/> to gather
    ///     incoming event data. The default implementation here simply forwards the incoming events to the Queue if they are
    ///     of the appropriate type.
    /// </summary>
    /// <typeparam name="T">The BrowserEvent type to produce.</typeparam>
    public abstract class WebBrowserEventProducer<T> : EventQueue<T>, IWebBrowserEventObserver where T : WebBrowserEvent
    {
        internal WebBrowserEventProducer() : base(new BoundedMultiConsumerChannelStrategy<T>(128, null))
        {

        }

        public void Notify(WebBrowserEvent @event)
        {
            if (@event is T specificEvent)
                Enqueue(specificEvent);
        }

        /// <summary>
        ///     The BrowserEvent type to be handled by this producer.
        /// </summary>
        private Type HandledEventType => typeof(T);
    }
}
