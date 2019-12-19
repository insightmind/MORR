using System.Collections.Generic;

namespace MORR.Shared.Events.Queue
{
    public interface IEventQueueStorageStrategy<T> where T : Event
    {
        /// <summary>
        /// Asynchronously gets all events from this storage strategy.
        /// </summary>
        /// <returns>An asynchronous stream of events stored in the event queue</returns>
        IAsyncEnumerable<T> GetEvents();

        /// <summary>
        /// Enqueues a new event using the specified strategy.
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        void Enqueue(T @event);
    }
}