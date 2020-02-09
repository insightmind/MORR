using System.Collections.Generic;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a read-only queue for <see cref="Event" /> types intended for encoding.
    /// </summary>
    /// <typeparam name="T">The type of the event</typeparam>
    public interface IEncodableEventQueue<out T> where T : Event
    {
        /// <summary>
        ///     Asynchronously gets all events as concrete type <typeparamref name="T" />.
        /// </summary>
        /// <returns>A stream of <typeparamref name="T" /></returns>
        IAsyncEnumerable<T> GetEvents();
    }
}