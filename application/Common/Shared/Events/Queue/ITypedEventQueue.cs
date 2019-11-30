namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides read-write access to a queue of events as concrete type <typeparamref name="T" />
    /// </summary>
    /// <typeparam name="T">The concrete type of the event</typeparam>
    public interface ITypedEventQueue<T> : IReadOnlyTypedEventQueue<T> where T : Event
    {
        /// <summary>
        ///     Asynchronously enqueues a new event
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        void Enqueue(T @event);
    }
}