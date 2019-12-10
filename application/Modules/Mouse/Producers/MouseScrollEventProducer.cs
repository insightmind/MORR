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
    ///     Provides a single-writer-multiple-reader queue for MouseScrollEvent
    /// </summary>
    [Export(typeof(MouseScrollEventProducer))]
    [Export(typeof(EventQueue<MouseScrollEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class MouseScrollEventProducer : EventQueue<MouseScrollEvent>
    {
        /// <summary>
        ///     Asynchronously gets all mouse scroll events as MouseScrollEvent type
        /// </summary>
        /// <returns>A stream of MouseScrollEvent</returns>
        public override IAsyncEnumerable<MouseScrollEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new mouse scroll event
        /// </summary>
        /// <param name="event">The mouse scroll event to enqueue</param>
        protected override void Enqueue(MouseScrollEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}