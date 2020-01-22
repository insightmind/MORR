namespace MORR.Shared.Events.Queue.Strategy.MultiConsumer
{
    public class BoundedMultiConsumerEventQueue<TEvent> : EventQueue<TEvent> where TEvent : Event
    {
        public BoundedMultiConsumerEventQueue(int bufferCapacity = 1024, uint? maxConsumers = null)
            : base(new BoundedMultiConsumerChannelStrategy<TEvent>(bufferCapacity, maxConsumers))
        {
            // Nothing special to do anymore.
        }
    }
}
