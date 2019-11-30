using System.Collections.Generic;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides read-only access to a queue of events as common type <see cref="Event" />
    /// </summary>
    public interface IReadOnlyUntypedEventQueue
    {
        /// <summary>
        ///     Asynchronously gets all events as common type <see cref="Event" />
        /// </summary>
        /// <returns>A stream of <see cref="Event" /></returns>
        IAsyncEnumerable<Event> GetUntypedEvents();
    }
}