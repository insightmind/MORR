namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a write-only queue for <see cref="Event" /> types intended for decoding.
    /// </summary>
    /// <typeparam name="T">The type of the event</typeparam>
    public interface IDecodeableEventQueue<in T> where T : Event
    {
        /// <summary>
        ///     Asynchronously enqueues a new event.
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        void Enqueue(T @event);
    }
}