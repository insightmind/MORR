using MORR.Shared.Events.Queue.Strategy;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader event queue for <see cref="Event" /> types which may be deserialized for
    ///     processing.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public abstract class SupportDeserializationEventQueue<T>
        : ReadOnlyEventQueue<T>, ISupportDeserializationEventQueue<T> where T : Event
    {
        protected SupportDeserializationEventQueue(IEventQueueStorageStrategy<T> storageStrategy) : base(
            storageStrategy) { }

        public new void Enqueue(T @event)
        {
            base.Enqueue(@event);
        }
    }
}