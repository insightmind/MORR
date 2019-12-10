using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.Clipboard.Events;
using MORR.Shared.Events;
using System.Composition;

namespace MORR.Modules.Clipboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ClipboardInteractEvent
    /// </summary>
    [Export(typeof(ClipboardInteractEventProducer))]
    [Export(typeof(EventQueue<ClipboardInteractEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class ClipboardInteractEventProducer : EventQueue<ClipboardInteractEvent>
    {
        /// <summary>
        ///     Asynchronously gets all clipboard interaction events as ClipboardInteractEvent type
        /// </summary>
        /// <returns>A stream of ClipboardInteractEvent</returns>
        public override IAsyncEnumerable<ClipboardInteractEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new clipboard interaction event
        /// </summary>
        /// <param name="event">The clipboard interaction event to enqueue</param>
        protected override void Enqueue(ClipboardInteractEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}