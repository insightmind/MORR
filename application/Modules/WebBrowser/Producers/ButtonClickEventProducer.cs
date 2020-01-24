using System;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ButtonClickEvent
    /// </summary>
    [Export(typeof(ButtonClickEventProducer))]
    [Export(typeof(EventQueue<ButtonClickEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(IWebBrowserEventProducer))]
    public class ButtonClickEventProducer : EventQueue<ButtonClickEvent>, IWebBrowserEventProducer
    {
        public ButtonClickEventProducer() : base(new BoundedMultiConsumerChannelStrategy<ButtonClickEvent>(256, null))
        {

        }
        public void Notify(WebBrowserEvent @event)
        {
            if (@event is ButtonClickEvent buttonClickEvent)
                Enqueue(buttonClickEvent);
        }

        public Type HandledEventType => typeof(ButtonClickEvent);
    }
}