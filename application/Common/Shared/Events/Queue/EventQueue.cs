using System.Collections.Generic;
using MORR.Shared.Events.Queue.Strategy;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    /// Provides the basic structure of an EventQueue.
    /// </summary>
    /// <typeparam name="TEvent">The type of Events which can be queued</typeparam>
    public abstract class EventQueue<TEvent> where TEvent : Event
    {
        /// <summary>
        /// Describes whether the queue is currently enabled to queue new events or not.
        /// </summary>
        public bool IsClosed => storageStrategy.IsClosed;

        private readonly IEventQueueStorageStrategy<TEvent> storageStrategy;

        protected EventQueue(IEventQueueStorageStrategy<TEvent> storageStrategy) => this.storageStrategy = storageStrategy;

        /// <summary>
        ///     Asynchronously gets all events as concrete type <typeparamref name="TEvent" />
        /// </summary>
        /// <returns>A stream of <typeparamref name="T" /></returns>
        public IAsyncEnumerable<TEvent> GetEvents() => storageStrategy.GetEvents();

        /// <summary>
        ///     Asynchronously enqueues a new event.
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        public void Enqueue(TEvent @event) => storageStrategy?.Enqueue(@event);

        /// <summary>
        ///     Opens the EventQueue so new events can be queued.
        /// </summary>
        public void Open() => storageStrategy.Open();

        /// <summary>
        ///     Closes the EventQueue so no new event can be queued.
        /// </summary>
        public void Close() => storageStrategy.Close();
    }
}
