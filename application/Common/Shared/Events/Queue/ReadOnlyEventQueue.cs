using System.Collections.Generic;
using MORR.Shared.Events.Queue.Strategy;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a read-only event queue for <see cref="Event" /> types which should only be read.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public abstract class ReadOnlyEventQueue<T> : BaseEventQueue<T>, IReadOnlyEventQueue<T> where T : Event
    {
        protected ReadOnlyEventQueue(IEventQueueStorageStrategy<T> storageStrategy) : base(storageStrategy) { }
    }
}