using System.Collections.Generic;
using MORR.Shared.Events.Queue.Strategy;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    /// Provides the basic structure of an EventQueue. This is explicitly not using an interface
    /// so no consumptions are made for subclasses which use a limiting interface to only expose a
    /// certain amount of the methods and functionality.
    /// </summary>
    /// <typeparam name="TEvent">The type of Events which can be queued</typeparam>
    public abstract class BaseEventQueue<TEvent> where TEvent : Event
    {
        private readonly IEventQueueStorageStrategy<TEvent> storageStrategy;

        protected BaseEventQueue(IEventQueueStorageStrategy<TEvent> storageStrategy) => this.storageStrategy = storageStrategy;

        /// <summary>
        ///     Asynchronously gets all events as concrete type <typeparamref name="T" />
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
        public void Open() => storageStrategy?.Open();

        /// <summary>
        ///     Closes the EventQueue so no new event can be queued.
        /// </summary>
        public void Close() => storageStrategy?.Close();
    }
}
