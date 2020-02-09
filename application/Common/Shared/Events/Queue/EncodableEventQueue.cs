using MORR.Shared.Events.Queue.Strategy;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a read-only event queue for <see cref="Event" /> types intended for encoding.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public abstract class EncodableEventQueue<T> : BaseEventQueue<T>, IEncodableEventQueue<T> where T : Event
    {
        protected EncodableEventQueue(IEventQueueStorageStrategy<T> storageStrategy) : base(storageStrategy) { }
    }
}