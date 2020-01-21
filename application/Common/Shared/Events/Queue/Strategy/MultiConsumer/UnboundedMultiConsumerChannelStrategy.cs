using System.Threading.Channels;

namespace MORR.Shared.Events.Queue.Strategy.MultiConsumer
{
    /// <summary>
    /// A BoundedMultiConsumerChannelStrategies is a Distributive-FIFO Queue which allows multiple producers and multiple consumers.
    /// However every event is propagated to each consumer once.
    /// 
    /// This queue is unbound, which means that an event is never dismissed.
    /// Make sure to only use this queue if you are sure it is bound by a maximum number of events.
    ///
    /// You can limit the number of consumers via the maxChannelConsumers argument.
    /// If you only have a single consumer, please use UnboundedSingleConsumerChannelStrategy.
    /// </summary>
    /// <typeparam name="TEvent">The type of event which is queued by the channel</typeparam>
    public class UnboundedMultiConsumerChannelStrategy<TEvent> : MultiConsumerChannelStrategy<TEvent> where TEvent : Event
    {
        private readonly UnboundedChannelOptions offeringOptions;
        private readonly UnboundedChannelOptions receivingOptions;

        /// <summary>
        /// Creates a new UnboundedMultiConsumerChannelStrategy.
        /// </summary>
        /// <param name="maxChannelConsumers">Maximum number of consumers allowed or null if unbound.</param>
        public UnboundedMultiConsumerChannelStrategy(uint? maxChannelConsumers)
        {
            offeringOptions = new UnboundedChannelOptions()
            {
                AllowSynchronousContinuations = true,
                SingleWriter = true,
                SingleReader = true
            };

            receivingOptions = new UnboundedChannelOptions()
            {
                AllowSynchronousContinuations = true,
                SingleWriter = false,
                SingleReader = true
            };

            StartReceiving(maxChannelConsumers);
        }

        protected override Channel<TEvent> CreateOfferingChannel()
        {
            return Channel.CreateUnbounded<TEvent>(offeringOptions);
        }

        protected override Channel<TEvent> CreateReceivingChannel()
        {
            return Channel.CreateUnbounded<TEvent>(receivingOptions);
        }
    }
}
