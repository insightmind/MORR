using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for TextSelectionEvent
    /// </summary>
    [Export(typeof(TextSelectionEventProducer))]
    [Export(typeof(EventQueue<TextSelectionEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class TextSelectionEventProducer : EventQueue<TextSelectionEvent>
    {
        /// <summary>
        ///     Asynchronously gets all text selection events as TextSelectionEvent type
        /// </summary>
        /// <returns>A stream of TextSelectionEvent</returns>
        public override IAsyncEnumerable<TextSelectionEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new text selection event
        /// </summary>
        /// <param name="event">The text selection event to enqueue</param>
        protected override void Enqueue(TextSelectionEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}