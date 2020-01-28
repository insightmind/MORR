using MORR.Shared.Events.Queue.Strategy.MultiConsumer;
using MORR.Shared.Events.Queue.Strategy.SingleConsumer;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides an event queue for the most common scenarios that supports deserialization.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public class DefaultEventQueue<T> : SupportDeserializationEventQueue<T> where T : Event
    {
        protected DefaultEventQueue(int bufferCapacity = 1024, uint? maxConsumers = null) :
            base(new BoundedMultiConsumerChannelStrategy<T>(bufferCapacity, maxConsumers)) { }
    }

    /// <summary>
    ///     Provides an event queue for events intended for encoding.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public class DefaultEncodeableEventQueue<T> : EncodeableEventQueue<T> where T : Event
    {
        public DefaultEncodeableEventQueue(int bufferCapacity = 1024) : base(
            new BoundedSingleConsumerChannelStrategy<T>(bufferCapacity)) { }
    }

    /// <summary>
    ///     Provides an event queue for events intended for decoding.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public class DefaultDecodeableEventQueue<T> : DecodeableEventQueue<T> where T : Event
    {
        public DefaultDecodeableEventQueue(int bufferCapacity = 1024) : base(
            new BoundedSingleConsumerChannelStrategy<T>(bufferCapacity)) { }
    }

    /// <summary>
    ///     Provides an event queue for events that do not support deserialization.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Event" /> in this queue.</typeparam>
    public class NonDeserializableEventQueue<T> : ReadOnlyEventQueue<T> where T : Event
    {
        public NonDeserializableEventQueue(int bufferCapacity = 1024, uint? maxConsumers = null) : base(
            new BoundedMultiConsumerChannelStrategy<T>(bufferCapacity, maxConsumers)) { }
    }

    public class DefaultTranscodeableEventQueue<T> : BoundedSingleConsumerTranscodeableEventQueue<T> where T : Event
    {
        public DefaultTranscodeableEventQueue(int bufferCapacity = 1024) : base(bufferCapacity) { }
    }

    public class BoundedMultiConsumerEventQueue<T> : EventQueueBase<T> where T : Event
    {
        public BoundedMultiConsumerEventQueue(int bufferCapacity = 1024, uint? maxConsumers = null)
            : base(new BoundedMultiConsumerChannelStrategy<T>(bufferCapacity, maxConsumers)) { }
    }

    public class BoundedSingleConsumerEventQueue<T> : EventQueueBase<T> where T : Event
    {
        public BoundedSingleConsumerEventQueue(int bufferCapcity = 1024) :
            base(new BoundedSingleConsumerChannelStrategy<T>(bufferCapcity)) { }
    }

    public class UnboundedMultiConsumerEventQueue<T> : EventQueueBase<T> where T : Event
    {
        public UnboundedMultiConsumerEventQueue(uint? maxConsumers = null) :
            base(new UnboundedMultiConsumerChannelStrategy<T>(maxConsumers)) { }
    }

    public class UnboundedSingleConsumerEventQueue<T> : EventQueueBase<T> where T : Event
    {
        public UnboundedSingleConsumerEventQueue() : base(new UnboundedSingleConsumerChannelStrategy<T>()) { }
    }

    public class BoundedMultiConsumerTranscodeableEventQueue<T> : TranscodeableEventQueueBase<T> where T : Event
    {
        public BoundedMultiConsumerTranscodeableEventQueue(int bufferCapacity = 1024, uint? maxConsumers = null) :
            base(new BoundedMultiConsumerChannelStrategy<T>(bufferCapacity, maxConsumers)) { }
    }

    public class BoundedSingleConsumerTranscodeableEventQueue<T> : TranscodeableEventQueueBase<T> where T : Event
    {
        public BoundedSingleConsumerTranscodeableEventQueue(int bufferCapacity = 1024) :
            base(new BoundedSingleConsumerChannelStrategy<T>(bufferCapacity)) { }
    }

    public class UnboundedMultiConsumerTranscodeableEventQueue<T> : TranscodeableEventQueueBase<T> where T : Event
    {
        public UnboundedMultiConsumerTranscodeableEventQueue(uint? maxConsumers) :
            base(new UnboundedMultiConsumerChannelStrategy<T>(maxConsumers)) { }
    }

    public class UnboundedSingleConsumerTranscodeableEventQueue<T> : TranscodeableEventQueueBase<T> where T : Event
    {
        public UnboundedSingleConsumerTranscodeableEventQueue() :
            base(new UnboundedSingleConsumerChannelStrategy<T>()) { }
    }
}