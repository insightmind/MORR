namespace MORR.Shared.Events.Queue.Strategy.SingleConsumer
{
    public class BoundedSingleConsumerEventQueue<TEvent> : EventQueue<TEvent> where TEvent : Event
    {
        public BoundedSingleConsumerEventQueue(int bufferCapacity = 1024)
            : base(new BoundedSingleConsumerChannelStrategy<TEvent>(bufferCapacity))
        {
            // Nothing special to do anymore.
        }
    }
}
