namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for <see cref="Event" /> types.
    /// </summary>
    /// <typeparam name="T">The type of the event</typeparam>
    public interface IReadWriteEventQueue<T> : IReadOnlyEventQueue<T> where T : Event
    {
        /// <summary>
        ///     Asynchronously enqueues a new event.
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        void Enqueue(T @event);
    }
}