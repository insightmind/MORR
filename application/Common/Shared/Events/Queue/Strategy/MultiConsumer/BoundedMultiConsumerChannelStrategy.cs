using System.Threading.Channels;

namespace MORR.Shared.Events.Queue.Strategy.MultiConsumer
{
    /// <summary>
    /// A BoundedMultiConsumerChannelStrategies is a Distributive-FIFO Queue which allows multiple producers and multiple consumers.
    /// However every event is propagated to each consumer once.
    /// This queue is bounded by a buffer capacity. To be more detailed each channel is bound by the bufferCapacity.
    /// Is the capacity reached for a specific channel it will drop the oldest event and write the new event.
    /// Therefore you can calculate the maximum capacity of this queue by:
    /// 
    /// FullCapacity = (NumOfConsumers + 1) * bufferCapacity
    ///
    /// You can limit the number of consumers via the maxChannelConsumers argument.
    /// If you only have a single consumer, please use BoundedSingleConsumerChannelStrategy.
    /// </summary>
    /// <typeparam name="TEvent">The type of event which is queued by the channel</typeparam>
    public class BoundedMultiConsumerChannelStrategy<TEvent> : MultiConsumerChannelStrategy<TEvent> where TEvent : Event
    {
        private readonly BoundedChannelOptions offeringOptions;
        private readonly BoundedChannelOptions receivingOptions;

        /// <summary>
        /// Creates a new BoundedMultiConsumerChannelStrategy with a given capacity.
        /// </summary>
        /// <param name="bufferCapacity">Capacity of the queue. Defaults: 1024</param>
        /// <param name="maxChannelConsumers">Maximum number of consumers allowed or null if unbound. Defaults: null</param>
        public BoundedMultiConsumerChannelStrategy(int bufferCapacity = 1024, uint? maxChannelConsumers = null)
        {
            offeringOptions = new BoundedChannelOptions(bufferCapacity)
            {
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleWriter = true,
                SingleReader = true
            };

            receivingOptions = new BoundedChannelOptions(bufferCapacity)
            {
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleWriter = false,
                SingleReader = true
            };

            StartReceiving(maxChannelConsumers);
        }

        protected override Channel<TEvent> CreateOfferingChannel()
        {
            return Channel.CreateBounded<TEvent>(offeringOptions);
        }

        protected override Channel<TEvent> CreateReceivingChannel()
        {
            return Channel.CreateBounded<TEvent>(receivingOptions);
        }
    }
}
