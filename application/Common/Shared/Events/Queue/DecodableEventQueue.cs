using MORR.Shared.Events.Queue.Strategy;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a read-only event queue for <see cref="Event" /> types intended for decoding.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public abstract class DecodableEventQueue<T> : BaseEventQueue<T>, IDecodableEventQueue<T> where T : Event
    {
        protected DecodableEventQueue(IEventQueueStorageStrategy<T> storageStrategy) : base(storageStrategy) { }
    }
}