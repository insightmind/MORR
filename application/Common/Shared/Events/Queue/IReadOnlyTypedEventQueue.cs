using System.Collections.Generic;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides read-only access to a queue of events as concrete type <typeparamref name="T" />
    /// </summary>
    /// <typeparam name="T">The concrete type of the event</typeparam>
    public interface IReadOnlyTypedEventQueue<out T> where T : Event
    {
        /// <summary>
        ///     Asynchronously gets all events as concrete type <typeparamref name="T" />
        /// </summary>
        /// <returns>A stream of <typeparamref name="T" /></returns>
        IAsyncEnumerable<T> GetTypedEvents();
    }
}