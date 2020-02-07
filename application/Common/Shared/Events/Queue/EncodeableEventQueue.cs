using System.Collections.Generic;
using MORR.Shared.Events.Queue.Strategy;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a read-only event queue for <see cref="Event" /> types intended for encoding.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public abstract class EncodeableEventQueue<T> : IEncodeableEventQueue<T> where T : Event
    {
        private readonly IEventQueueStorageStrategy<T> storageStrategy;

        protected EncodeableEventQueue(IEventQueueStorageStrategy<T> storageStrategy)
        {
            this.storageStrategy = storageStrategy;
        }

        public IAsyncEnumerable<T> GetEvents()
        {
            return storageStrategy.GetEvents();
        }

        protected void Enqueue(T @event)
        {
            storageStrategy.Enqueue(@event);
        }

        protected void NotifyOnEnqueueFinished()
        {
            storageStrategy.NotifyOnEnqueueFinished();
        }
    }
}