using System.Threading.Channels;

namespace MORR.Shared.Events.Queue.Strategy.SingleConsumer
{
    /// <summary>
    /// A BoundedSingleConsumerChannelStrategy is a Distributive-FIFO Queue which allows multiple producers and a single consumer.
    /// This queue is performance optimized for a single consumer and should be preferred in this case.
    /// Please make sure only a single consumer will request. Otherwise an exception is thrown.
    ///
    /// This queue is bounded by a buffer capacity. If capacity is reached it will dismiss the oldest event and queue the new event.
    ///
    /// </summary>
    /// <typeparam name="TEvent">The type of event which is queued by the channel</typeparam>
    public class BoundedSingleConsumerChannelStrategy<TEvent> : SingleConsumerChannelStrategy<TEvent> where TEvent : Event
    {
        private readonly BoundedChannelOptions options;

        /// <summary>
        /// Creates a new BoundedSingleConsumerChannelStrategy with a given capacity.
        /// </summary>
        /// <param name="bufferCapacity">Capacity of the queue. Defaults: 1024</param>
        public BoundedSingleConsumerChannelStrategy(int bufferCapacity = 1024)
        {
            options = new BoundedChannelOptions(bufferCapacity)
            {
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleWriter = true,
                SingleReader = true
            };

            StartReceiving();
        }

        protected override Channel<TEvent> CreateChannel()
        {
            return Channel.CreateBounded<TEvent>(options);
        }
    }
}
