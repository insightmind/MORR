using System.Collections.Generic;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for <see cref="Event" /> types.
    /// </summary>
    /// <typeparam name="T">The type of the event</typeparam>
    public abstract class EventQueueBase<T> : IReadWriteEventQueue<T> where T : Event
    {
        private readonly IEventQueueStorageStrategy<T> storageStrategy;

        protected EventQueueBase(IEventQueueStorageStrategy<T> storageStrategy)
        {
            this.storageStrategy = storageStrategy;
        }

        /// <summary>
        ///     Asynchronously gets all events as concrete type <typeparamref name="T" />.
        /// </summary>
        /// <returns>A stream of <typeparamref name="T" /></returns>
        public IAsyncEnumerable<T> GetEvents()
        {
            return storageStrategy.GetEvents();
        }

        /// <summary>
        ///     Asynchronously enqueues a new event.
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        public void Enqueue(T @event)
        {
            storageStrategy.Enqueue(@event);
        }
    }

    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for <see cref="Event" /> types intended for transcoding.
    /// </summary>
    /// <typeparam name="T">The type of the event</typeparam>
    public abstract class TranscodeableEventQueueBase<T> : ITranscodeableEventQueue<T> where T : Event
    {
        private readonly IEventQueueStorageStrategy<T> storageStrategy;

        protected TranscodeableEventQueueBase(IEventQueueStorageStrategy<T> storageStrategy)
        {
            this.storageStrategy = storageStrategy;
        }

        /// <summary>
        ///     Asynchronously gets all events as concrete type <typeparamref name="T" />.
        /// </summary>
        /// <returns>A stream of <typeparamref name="T" /></returns>
        public IAsyncEnumerable<T> GetEvents()
        {
            return storageStrategy.GetEvents();
        }

        /// <summary>
        ///     Asynchronously enqueues a new event.
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        public void Enqueue(T @event)
        {
            storageStrategy.Enqueue(@event);
        }
    }
}