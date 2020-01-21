using System.Threading.Channels;

namespace MORR.Shared.Events.Queue.Strategy.SingleConsumer
{
    /// <summary>
    /// A BoundedSingleConsumerChannelStrategy is a Distributive-FIFO Queue which allows multiple producers and a single consumer.
    /// This queue is performance optimized for a single consumer and should be preferred in this case.
    /// Please make sure only a single consumer will request. Otherwise an exception is thrown.
    ///
    /// This queue is unbounded and will therefore don't dismiss any events. Please make sure that the num of events is bounded by a maximum number. 
    ///
    /// </summary>
    /// <typeparam name="TEvent">The type of event which is queued by the channel</typeparam>
    public class UnboundedSingleConsumerChannelStrategy<TEvent> : SingleConsumerChannelStrategy<TEvent> where TEvent : Event
    {
        private readonly UnboundedChannelOptions options;

        /// <summary>
        /// Creates a new UnboundedSingleConsumerChannelStrategy.
        /// </summary>
        public UnboundedSingleConsumerChannelStrategy()
        {
            options = new UnboundedChannelOptions()
            {
                AllowSynchronousContinuations = true,
                SingleWriter = false,
                SingleReader = true
            };

            StartReceiving();
        }

        protected override Channel<TEvent> CreateChannel()
        {
            return Channel.CreateUnbounded<TEvent>(options);
        }
    }
}
