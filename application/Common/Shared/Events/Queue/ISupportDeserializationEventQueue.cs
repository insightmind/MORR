using System;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for <see cref="Event" /> types with support for deserialization.
    /// </summary>
    /// <typeparam name="T">The type of the event</typeparam>
    public interface ISupportDeserializationEventQueue<out T>: IReadOnlyEventQueue<T> where T : Event
    {
        /// <summary>
        ///     Asynchronously enqueues a new event.
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        void Enqueue(object @event);

        /// <summary>
        ///     Opens the EventQueue so new events can be queued.
        /// </summary>
        public void Open();

        /// <summary>
        ///     Closes the EventQueue so no new event can be queued.
        /// </summary>
        public void Close();
    }
}