using System;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for <see cref="Event" /> types with support for deserialization.
    /// </summary>
    /// <typeparam name="T">The type of the event</typeparam>
    public interface ISupportDeserializationEventQueue<out T> where T : Event
    {
        /// <summary>
        /// Describes whether the queue is currently enabled to queue new events or not.
        /// </summary>
        public bool IsClosed { get; }

        /// <summary>
        ///     The actual type of the events queued in this queue.
        /// </summary>
        Type EventType => typeof(T);

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
        void Close();
    }
}