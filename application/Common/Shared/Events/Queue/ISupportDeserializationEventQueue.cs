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
        ///     The actual type of the events enqueued in this queue.
        /// </summary>
        Type EventType => typeof(T);

        /// <summary>
        ///     Asynchronously enqueues a new event.
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        void Enqueue(object @event);

        /// <summary>
        ///     Notifies the event queue that no more events will be enqueued.
        /// </summary>
        void NotifyOnEnqueueFinished();
    }
}