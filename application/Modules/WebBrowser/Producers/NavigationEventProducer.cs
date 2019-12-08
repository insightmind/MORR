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
    ///     Provides a single-writer-multiple-reader queue for NavigationEvent
    /// </summary>
    [Export(typeof(NavigationEventProducer))]
    [Export(typeof(EventQueue<NavigationEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class NavigationEventProducer : EventQueue<NavigationEvent>
    {
        /// <summary>
        ///     Asynchronously gets all navigation events as NavigationEvent type
        /// </summary>
        /// <returns>A stream of NavigationEvent</returns>
        public override IAsyncEnumerable<NavigationEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new navigation event
        /// </summary>
        /// <param name="event">The navigation event to enqueue</param>
        protected override void Enqueue(NavigationEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}