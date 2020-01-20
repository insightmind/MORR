namespace MORR.Shared.Events.Queue.Strategy.MultiConsumer
{
    public class BoundedMultiConsumerEventQueue<TEvent> : EventQueue<TEvent> where TEvent : Event
    {
        public BoundedMultiConsumerEventQueue(int bufferCapacity, uint? maxConsumers = null)
            : base(new BoundedMultiConsumerChannelStrategy<TEvent>(bufferCapacity, maxConsumers))
        {
            // Nothing special to do anymore.
        }
    }
}
