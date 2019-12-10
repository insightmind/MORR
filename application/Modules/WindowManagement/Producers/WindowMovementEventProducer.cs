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
    ///     Provides a single-writer-multiple-reader queue for WindowMovementEvent
    /// </summary>
    [Export(typeof(WindowMovementEventProducer))]
    [Export(typeof(EventQueue<WindowMovementEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class WindowMovementEventProducer : EventQueue<WindowMovementEvent>
    {
        /// <summary>
        ///     Asynchronously gets all window movement events as WindowMovementEvent type
        /// </summary>
        /// <returns>A stream of WindowMovementEvent</returns>
        public override IAsyncEnumerable<WindowMovementEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new window movement event
        /// </summary>
        /// <param name="event">The window movement event to enqueue</param>
        protected override void Enqueue(WindowMovementEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}