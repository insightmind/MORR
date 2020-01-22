namespace MORR.Shared.Events.Queue.Strategy.MultiConsumer
{
    public class UnboundedMultiConsumerEventQueue<TEvent> : EventQueue<TEvent> where TEvent : Event
    {
        public UnboundedMultiConsumerEventQueue(uint? maxConsumers = null)
            : base(new UnboundedMultiConsumerChannelStrategy<TEvent>(maxConsumers))
        {
            // Nothing special to do anymore.
        }
    }
}
