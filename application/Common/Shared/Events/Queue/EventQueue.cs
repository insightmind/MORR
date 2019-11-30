using System.Collections.Generic;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for <see cref="Event" /> types
    /// </summary>
    /// <typeparam name="T">The type of the event</typeparam>
    public abstract class EventQueue<T> : ITypedEventQueue<T>, IReadOnlyUntypedEventQueue where T : Event
    {
        /// <summary>
        ///     Asynchronously gets all events as common type <see cref="Event" />
        /// </summary>
        /// <returns>A stream of <see cref="Event" /></returns>
        public abstract IAsyncEnumerable<Event> GetUntypedEvents();

        /// <summary>
        ///     Asynchronously gets all events as concrete type <typeparamref name="T" />
        /// </summary>
        /// <returns>A stream of <typeparamref name="T" /></returns>
        public abstract IAsyncEnumerable<T> GetTypedEvents();

        /// <summary>
        ///     Asynchronously enqueues a new event
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        public abstract void Enqueue(T @event);
    }
}