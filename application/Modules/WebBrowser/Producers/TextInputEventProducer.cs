using System;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for TextInputEvent
    /// </summary>
    [Export(typeof(TextInputEventProducer))]
    [Export(typeof(EventQueue<TextInputEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(IWebBrowserEventProducer))]
    public class TextInputEventProducer : EventQueue<TextInputEvent>, IWebBrowserEventProducer
    {
        public TextInputEventProducer() : base(new BoundedMultiConsumerChannelStrategy<TextInputEvent>(64, null))
        {

        }

        public void Notify(WebBrowserEvent @event)
        {
            if (@event is TextInputEvent textInputEvent)
                Enqueue(textInputEvent);
        }

        public Type HandledEventType => typeof(TextInputEvent);
    }
}