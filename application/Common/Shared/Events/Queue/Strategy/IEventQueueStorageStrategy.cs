using System.Collections.Generic;
using System.Threading;

namespace MORR.Shared.Events.Queue.Strategy
{
    public interface IEventQueueStorageStrategy<T> where T : Event
    {
        /// <summary>
        /// Describes whether the queue is currently enabled to queue new events or not.
        /// </summary>
        bool IsClosed { get; }

        /// <summary>
        /// Asynchronously gets all events from this storage strategy.
        /// </summary>
        /// <returns>An asynchronous stream of events stored in the event queue</returns>
        IAsyncEnumerable<T> GetEvents(CancellationToken token = default);

        /// <summary>
        /// Enqueues a new event using the specified strategy.
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        void Enqueue(T @event);

        /// <summary>
        /// Notifies the event queue that it is free to accept input.
        /// </summary>
        void Open();

        /// <summary>
        /// Notifies the event queue that no more events will be queued.
        /// </summary>
        void Close();
    }
}