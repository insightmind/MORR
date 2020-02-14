using System;
using MORR.Shared.Events.Queue.Strategy;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader event queue for <see cref="Event" /> types which may be deserialized for
    ///     processing.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public abstract class SupportDeserializationEventQueue<T> : BaseEventQueue<T>, IReadOnlyEventQueue<T>, ISupportDeserializationEventQueue<T> where T : Event
    {
        protected SupportDeserializationEventQueue(IEventQueueStorageStrategy<T> storageStrategy) : base(storageStrategy) { }

        public void Enqueue(object @event)
        {
            if (!(@event is T typedEvent))
            {
                throw new ArgumentException($"Cannot convert event type {@event.GetType()} to expected type {typeof(T)}.");
            }

            base.Enqueue(typedEvent);
        }
    }
}