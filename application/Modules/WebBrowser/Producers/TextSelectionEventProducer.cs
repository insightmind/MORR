using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for TextSelectionEvent
    /// </summary>
    [Export(typeof(TextSelectionEventProducer))]
    [Export(typeof(EventQueue<TextSelectionEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(IWebBrowserEventProducer))]
    public class TextSelectionEventProducer : EventQueue<TextSelectionEvent>, IWebBrowserEventProducer
    {
        public TextSelectionEventProducer() : base(
            new BoundedMultiConsumerChannelStrategy<TextSelectionEvent>(16, null))
        {

        }

        public void Notify(WebBrowserEvent @event)
        {
            if (@event is TextSelectionEvent textSelectionEvent)
                Enqueue(textSelectionEvent);
        }

        public Type HandledEventType => typeof(TextSelectionEvent);
    }
}