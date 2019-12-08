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
    ///     Provides a single-writer-multiple-reader queue for OpenTabEvent
    /// </summary>
    [Export(typeof (OpenTabEventProducer))]
    [Export(typeof(EventQueue<OpenTabEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class OpenTabEventProducer : EventQueue<OpenTabEvent>
    {
        /// <summary>
        ///     Asynchronously gets all open tab events as OpenTabEvent type
        /// </summary>
        /// <returns>A stream of OpenTabEvent</returns>
        public override IAsyncEnumerable<OpenTabEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new open tab event
        /// </summary>
        /// <param name="event">The open tab event to enqueue</param>
        protected override void Enqueue(OpenTabEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
