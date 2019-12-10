using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.Keyboard.Events;
using MORR.Shared.Events;
using System.Composition;

namespace MORR.Modules.Keyboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for KeyboardInteractEvent
    /// </summary>
    [Export(typeof(KeyboardInteractEventProducer))]
    [Export(typeof(EventQueue<KeyboardInteractEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class KeyboardInteractEventProducer : EventQueue<KeyboardInteractEvent>
    {
        /// <summary>
        ///     Asynchronously gets all keyboard interaction events as KeyboardInteractEvent type
        /// </summary>
        /// <returns>A stream of KeyboardInteractEvent</returns>
        public override IAsyncEnumerable<KeyboardInteractEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new keyboard interaction event
        /// </summary>
        /// <param name="event">The keyboard interaction event to enqueue</param>
        protected override void Enqueue(KeyboardInteractEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}