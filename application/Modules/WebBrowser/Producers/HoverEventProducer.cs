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
    ///     Provides a single-writer-multiple-reader queue for HoverEvent
    /// </summary>
    [Export(typeof(HoverEventProducer))]
    [Export(typeof(EventQueue<HoverEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class HoverEventProducer : EventQueue<HoverEvent>
    {
        /// <summary>
        ///     Asynchronously gets all hover events as HoverEvent type
        /// </summary>
        /// <returns>A stream of HoverEvent</returns>
        public override IAsyncEnumerable<HoverEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new hover event
        /// </summary>
        /// <param name="event">The hover event to enqueue</param>
        protected override void Enqueue(HoverEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}