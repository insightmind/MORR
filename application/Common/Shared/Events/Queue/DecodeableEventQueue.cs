using System.Collections.Generic;
using System.Threading;
using MORR.Shared.Events.Queue.Strategy;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a read-only event queue for <see cref="Event" /> types intended for decoding.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public abstract class DecodeableEventQueue<T> : IDecodeableEventQueue<T> where T : Event
    {
        private readonly IEventQueueStorageStrategy<T> storageStrategy;

        protected DecodeableEventQueue(IEventQueueStorageStrategy<T> storageStrategy)
        {
            this.storageStrategy = storageStrategy;
        }

        public IAsyncEnumerable<T> GetEvents(CancellationToken cancellationToken = default)
        {
            return storageStrategy.GetEvents(cancellationToken);
        }

        protected void Enqueue(T @event)
        {
            storageStrategy.Enqueue(@event);
        }
    }
}