using MORR.Shared.Events.Queue.Strategy.MultiConsumer;
using MORR.Shared.Events.Queue.Strategy.SingleConsumer;

namespace MORR.Shared.Events.Queue
{
    public class DefaultEventQueue<T> : BoundedMultiConsumerEventQueue<T> where T : Event
    {
        public DefaultEventQueue(int bufferCapacity = 1024, uint? maxConsumers = null) :
            base(bufferCapacity, maxConsumers) { }
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