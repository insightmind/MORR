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
    ///     Provides a single-writer-multiple-reader queue for MouseClickEvent
    /// </summary>
    [Export(typeof(MouseClickEventProducer))]
    [Export(typeof(EventQueue<MouseClickEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class MouseClickEventProducer : EventQueue<MouseClickEvent>
    {
        /// <summary>
        ///     Asynchronously gets all mouse click events as MouseClickEvent type
        /// </summary>
        /// <returns>A stream of MouseClickEvent</returns>
        public override IAsyncEnumerable<MouseClickEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new mouse click event
        /// </summary>
        /// <param name="event">The mouse click event to enqueue</param>
        protected override void Enqueue(MouseClickEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}