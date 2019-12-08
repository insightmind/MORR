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
    ///     Provides a single-writer-multiple-reader queue for TextInputEvent
    /// </summary>
    [Export(typeof(TextInputEventProducer))]
    [Export(typeof(EventQueue<TextInputEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class TextInputEventProducer : EventQueue<TextInputEvent>
    {
        /// <summary>
        ///     Asynchronously gets all text input events as TexpInputEvent type
        /// </summary>
        /// <returns>A stream of TextInputEvent</returns>
        public override IAsyncEnumerable<TextInputEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new text input event
        /// </summary>
        /// <param name="event">The text input event to enqueue</param>
        protected override void Enqueue(TextInputEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}