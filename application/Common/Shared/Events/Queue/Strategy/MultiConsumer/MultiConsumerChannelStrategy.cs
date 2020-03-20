using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MORR.Shared.Events.Queue.Strategy.MultiConsumer
{
    /// <summary>
    /// A MultiConsumerChannelStrategies is a Distributive-FIFO Queue which allows multiple producers and multiple consumers.
    /// However every event is propagated to each consumer once.
    /// </summary>
    /// <typeparam name="TEvent">The type of event which is queued by the channel</typeparam>
    public abstract class MultiConsumerChannelStrategy<TEvent> : IEventQueueStorageStrategy<TEvent> where TEvent : Event
    {
        private uint? maxChannelConsumers;
        private Channel<TEvent, TEvent>? receivingChannel;
        private readonly List<Channel<TEvent>> offeringChannels = new List<Channel<TEvent>>();
        private readonly Mutex subscriptionMutex = new Mutex();
        private const int timeOut = 500;

        protected void StartReceiving(uint? maxChannelConsumers)
        {
            if (maxChannelConsumers == 1)
            {
                throw new ChannelConsumingException("ERROR: You are using MultiConsumerChannel with a max consumer of 1. " +
                                                    "Please change to a SingleConsumerChannel for maximum performance!");
            }
            else if (maxChannelConsumers == 0)
            {
                throw new ChannelConsumingException("ERROR: You are using a channel strategy that disallows consuming. This is invalid!");
            }

            this.maxChannelConsumers = maxChannelConsumers;
        }

        public bool IsClosed { get; private set; } = true;

        /// <summary>
        ///     Asynchronously gets all events as concrete type <typeparamref name="T" />.
        /// </summary>
        /// <returns>A stream of <typeparamref name="T" /></returns>
        public IAsyncEnumerable<TEvent> GetEvents(CancellationToken token = default)
        {
            subscriptionMutex.WaitOne(timeOut);

            if ((maxChannelConsumers != null) && (offeringChannels.Count >= maxChannelConsumers))
            {
                subscriptionMutex.ReleaseMutex();
                throw new ChannelConsumingException($"Maximum number ({maxChannelConsumers}) of consumers reached!");
            }

            var channel = CreateOfferingChannel();
            offeringChannels?.Add(channel);
            token.Register(FreeChannel, channel);

            subscriptionMutex.ReleaseMutex();
            return channel.Reader.ReadAllAsync(token);
        }

        /// <summary>
        ///     Asynchronously enqueues a new event.
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        public async void Enqueue(TEvent @event)
        {
            if (receivingChannel == null || IsClosed) return;
            await EnqueueAsync(receivingChannel, @event);
        }

        public void Open()
        {
            subscriptionMutex.WaitOne(timeOut);

            if (!IsClosed)
            {
                subscriptionMutex.ReleaseMutex();
                return;
            }

            offeringChannels.Clear();
            receivingChannel = CreateReceivingChannel();
            _ = DistributeEventsAsync();
            IsClosed = false;

            subscriptionMutex.ReleaseMutex();
        }

        public void Close()
        {
            subscriptionMutex.WaitOne(timeOut);

            if (IsClosed)
            {
                subscriptionMutex.ReleaseMutex();
                return;
            }

            IsClosed = true;
            subscriptionMutex.ReleaseMutex();

            receivingChannel?.Writer.TryComplete();

            foreach (var channel in offeringChannels)
            {
                channel?.Writer.TryComplete();
            }
        }

        private static ValueTask<bool> EnqueueAsync(Channel<TEvent, TEvent> channel, TEvent @event)
        {
            async Task<bool> AsyncSlowPath(TEvent item)
            {
                while (await channel.Writer.WaitToWriteAsync())
                {
                    if (channel.Writer.TryWrite(item)) return true;
                }
                return false; // Channel was completed during the wait
            }

            return channel.Writer.TryWrite(@event) ? new ValueTask<bool>(true) : new ValueTask<bool>(AsyncSlowPath(@event));
        }

        private async Task DistributeEventsAsync()
        {
            if (receivingChannel == null) return;

            await foreach (var @event in receivingChannel.Reader.ReadAllAsync())
            {
                foreach (var channel in offeringChannels)
                {
                    _ = EnqueueAsync(channel, @event);
                }
            }
        }

        public void FreeChannel(object? channelObject)
        {
            if (!(channelObject is Channel<TEvent> channel)) return;

            subscriptionMutex.WaitOne(timeOut);
            offeringChannels?.Remove(channel);
            subscriptionMutex.ReleaseMutex();
        }

        protected abstract Channel<TEvent> CreateOfferingChannel();
        protected abstract Channel<TEvent> CreateReceivingChannel();
    }
}
