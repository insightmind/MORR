using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WindowManagement.Events;
using MORR.Shared.Events;
using System.Composition;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowResizingEvent
    /// </summary>
    [Export(typeof(WindowResizingEventProducer))]
    [Export(typeof(EventQueue<WindowResizingEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class WindowResizingEventProducer : EventQueue<WindowResizingEvent>
    {
        /// <summary>
        ///     Asynchronously gets all window resizing events as WindowResizingEvent type
        /// </summary>
        /// <returns>A stream of WindowResiznigEvent</returns>
        public override IAsyncEnumerable<WindowResizingEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new window resizing event
        /// </summary>
        /// <param name="event">The window resizing event to enqueue</param>
        protected override void Enqueue(WindowResizingEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}