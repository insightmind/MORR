using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public abstract class MultiConsumerChannelStrategy<TEvent>: IEventQueueStorageStrategy<TEvent> where TEvent: Event
    {
        private uint? maxChannelConsumers = null;
        private Channel<TEvent> receivingChannel;
        private readonly List<Channel<TEvent>> offeringChannels = new List<Channel<TEvent>>();

        protected async void StartReceiving(uint? maxChannelConsumers)
        {
            if (maxChannelConsumers == 1)
            { 
                Console.WriteLine("WARNING: You are using MultiConsumerChannel with a max consumer of 1. " +
                                "Please change to a SingleConsumerChannel for maximum performance!");
            } else if (maxChannelConsumers == 0)
            {
                throw new ChannelConsumingException("ERROR: You are using a channel strategy that disallows consuming. This is invalid!");
            }

            this.maxChannelConsumers = maxChannelConsumers;
            receivingChannel = CreateReceivingChannel();
            await DistributeEventsAsync();
        }

        /// <summary>
        ///     Asynchronously gets all events as concrete type <typeparamref name="T" />.
        /// </summary>
        /// <returns>A stream of <typeparamref name="T" /></returns>
        public IAsyncEnumerable<TEvent> GetEvents([EnumeratorCancellation] CancellationToken token = default)
        {
            var channel = CreateOfferingChannel();
            offeringChannels.Add(channel);
            return channel.Reader.ReadAllAsync(token);
        }

        /// <summary>
        ///     Asynchronously enqueues a new event.
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        public async void Enqueue(TEvent @event)
        {
            await EnqueueAsync(receivingChannel, @event);
        }

        private ValueTask EnqueueAsync(Channel<TEvent> channel, TEvent @event)
        {
            async Task AsyncSlowPath(TEvent @event)
            {
                await channel.Writer.WriteAsync(@event);
            }

            return channel.Writer.TryWrite(@event) ? default : new ValueTask(AsyncSlowPath(@event));
        }

        private async Task DistributeEventsAsync()
        {
            await foreach (var @event in receivingChannel.Reader.ReadAllAsync())
            {
                foreach (var channel in offeringChannels)
                {
                    EnqueueAsync(channel, @event);
                }
            }
        }
        protected abstract Channel<TEvent> CreateOfferingChannel();
        protected abstract Channel<TEvent> CreateReceivingChannel();
    }
}
