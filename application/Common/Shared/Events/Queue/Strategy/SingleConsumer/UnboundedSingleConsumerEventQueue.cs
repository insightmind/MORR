namespace MORR.Shared.Events.Queue.Strategy.SingleConsumer
{
    public class UnboundedSingleConsumerEventQueue<TEvent> : EventQueue<TEvent> where TEvent : Event
    {
        public UnboundedSingleConsumerEventQueue()
            : base(new UnboundedSingleConsumerChannelStrategy<TEvent>())
        {
            // Nothing special to do anymore.
        }
    }
}
