using MORR.Shared.Events.Queue.Strategy.MultiConsumer;
using MORR.Shared.Events.Queue.Strategy.SingleConsumer;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides an event queue for the most common scenarios that supports deserialization.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public abstract class DefaultEventQueue<T> : SupportDeserializationEventQueue<T> where T : Event
    {
        protected DefaultEventQueue(int bufferCapacity = 1024, uint? maxConsumers = null) :
            base(new BoundedMultiConsumerChannelStrategy<T>(bufferCapacity, maxConsumers)) { }
    }

    /// <summary>
    ///     Provides an event queue for events intended for encoding.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public abstract class DefaultEncodableEventQueue<T> : EncodableEventQueue<T> where T : Event
    {
        protected DefaultEncodableEventQueue(int bufferCapacity = 1024) : base(
            new BoundedSingleConsumerChannelStrategy<T>(bufferCapacity)) { }
    }

    /// <summary>
    ///     Provides an event queue for events intended for decoding.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public abstract class DefaultDecodableEventQueue<T> : DecodableEventQueue<T> where T : Event
    {
        protected DefaultDecodableEventQueue(int bufferCapacity = 1024, uint? maxConsumers = null) : base(
            new BoundedMultiConsumerChannelStrategy<T>(bufferCapacity, maxConsumers)) { }
    }

    /// <summary>
    ///     Provides an event queue for events that do not support deserialization.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public abstract class NonDeserializableEventQueue<T> : ReadOnlyEventQueue<T> where T : Event
    {
        protected NonDeserializableEventQueue(int bufferCapacity = 1024, uint? maxConsumers = null) : base(
            new BoundedMultiConsumerChannelStrategy<T>(bufferCapacity, maxConsumers)) { }
    }
}