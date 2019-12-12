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
    ///     Provides a single-writer-multiple-reader queue for WindowFocusEvent
    /// </summary>
    [Export(typeof(WindowFocusEventProducer))]
    [Export(typeof(EventQueue<WindowFocusEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class WindowFocusEventProducer : EventQueue<WindowFocusEvent>
    {
        /// <summary>
        ///     Asynchronously gets all window focus events as WindowFocusEvent type
        /// </summary>
        /// <returns>A stream of WindowFocusEvent</returns>
        public override IAsyncEnumerable<WindowFocusEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new window focus event
        /// </summary>
        /// <param name="event">The window focus event to enqueue</param>
        protected override void Enqueue(WindowFocusEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}