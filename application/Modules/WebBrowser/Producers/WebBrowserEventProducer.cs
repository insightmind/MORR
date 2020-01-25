using System;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     A generic producer for WebBrowserEvents, which will need to be subscribed to a <see cref="IWebBrowserEventObservible"/> to gather
    ///     incoming event data. The default implementation here simply forwards the incoming events to the Queue if they are
    ///     of the appropriate type.
    /// </summary>
    /// <typeparam name="T">The BrowserEvent type to produce.</typeparam>
    public abstract class WebBrowserEventProducer<T> : DefaultEventQueue<T>, IWebBrowserEventObserver where T : WebBrowserEvent
    {

        /// <summary>
        ///     Simply forward the event to the internal queue if its of the appropriate type. Ignore otherwise.
        /// </summary>
        /// <param name="event"></param>
        public void Notify(WebBrowserEvent @event)
        {
            if (@event is T specificEvent)
                Enqueue(specificEvent);
        }

        /// <summary>
        ///     The BrowserEvent type to be handled by this producer.
        /// </summary>
        public Type HandledEventType => typeof(T);
    }
}
