namespace MORR.Shared.Events.Queue.Strategy.SingleConsumer
{
    public class BoundedSingleConsumerEventQueue<TEvent> : EventQueue<TEvent> where TEvent : Event
    {
        public BoundedSingleConsumerEventQueue(int bufferCapacity)
            : base(new BoundedSingleConsumerChannelStrategy<TEvent>(bufferCapacity))
        {
            // Nothing special to do anymore.
        }
    }
}
