using System;
using System.Collections.Generic;
using System.Composition;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for SwitchTabEvent
    /// </summary>
    [Export(typeof(SwitchTabEventProducer))]
    [Export(typeof(EventQueue<SwitchTabEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(IWebBrowserEventProducer))]
    public class SwitchTabEventProducer : EventQueue<SwitchTabEvent>, IWebBrowserEventProducer
    {
        public SwitchTabEventProducer() : base(new BoundedMultiConsumerChannelStrategy<SwitchTabEvent>(64, null))
        {

        }

        public void Notify(WebBrowserEvent @event)
        {
            if (@event is SwitchTabEvent switchTabEvent)
                Enqueue(switchTabEvent);
        }

        public Type HandledEventType => typeof(SwitchTabEvent);
    }
}
