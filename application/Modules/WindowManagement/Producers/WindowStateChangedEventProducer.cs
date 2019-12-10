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
    ///     Provides a single-writer-multiple-reader queue for WindowStateChangedEvent
    /// </summary>
    [Export(typeof(WindowStateChangedEventProducer))]
    [Export(typeof(EventQueue<WindowStateChangedEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class WindowStateChangedEventProducer : EventQueue<WindowStateChangedEvent>
    {
        /// <summary>
        ///     Asynchronously gets all window state changed events as WindowStateChangedEvent type
        /// </summary>
        /// <returns>A stream of WindowStateChangedEvent</returns>
        public override IAsyncEnumerable<WindowStateChangedEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new window state changed event
        /// </summary>
        /// <param name="event">The window state changed event to enqueue</param>
        protected override void Enqueue(WindowStateChangedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}