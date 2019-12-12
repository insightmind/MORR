using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events;
using System.Composition;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseMoveEvent
    /// </summary>
    [Export(typeof(MouseMoveEventProducer))]
    [Export(typeof(EventQueue<MouseMoveEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class MouseMoveEventProducer : EventQueue<MouseMoveEvent>
    {
        /// <summary>
        ///     Asynchronously gets all mouse move events as MouseMoveEvent type
        /// </summary>
        /// <returns>A stream of MouseMoveEvent</returns>
        public override IAsyncEnumerable<MouseMoveEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new mouse move event
        /// </summary>
        /// <param name="event">The mouse move event to enqueue</param>
        protected override void Enqueue(MouseMoveEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}