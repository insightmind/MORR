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
    ///     Provides a single-writer-multiple-reader queue for CloseTabEvent
    /// </summary>
    [Export(typeof(CloseTabEventProducer))]
    [Export(typeof(EventQueue<CloseTabEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class CloseTabEventProducer : EventQueue<CloseTabEvent>
    {
        /// <summary>
        ///     Asynchronously gets all close tab events as CloseTabEvent type
        /// </summary>
        /// <returns>A stream of CloseTabEvent</returns>
        public override IAsyncEnumerable<CloseTabEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new close tab event
        /// </summary>
        /// <param name="event">The close tab event to enqueue</param>
        protected override void Enqueue(CloseTabEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}