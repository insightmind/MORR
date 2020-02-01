using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MORR.Shared.Events.Queue.Strategy
{
    public interface IEventQueueStorageStrategy<T> where T : Event
    {
        /// <summary>
        /// Asynchronously gets all events from this storage strategy.
        /// </summary>
        /// <returns>An asynchronous stream of events stored in the event queue</returns>
        IAsyncEnumerable<T> GetEvents([EnumeratorCancellation] CancellationToken token = default);

        /// <summary>
        /// Enqueues a new event using the specified strategy.
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        void Enqueue(T @event);
    }
}