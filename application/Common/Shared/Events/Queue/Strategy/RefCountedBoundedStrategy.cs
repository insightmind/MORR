using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MORR.Shared.Events.Queue.Strategy
{
    public class RefCountedBoundedStrategy<TEvent> : IEventQueueStorageStrategy<TEvent> where TEvent : Event
    {
        private readonly BoundedChannelOptions offeringOptions;
        private readonly BoundedChannelOptions receivingOptions;

        private readonly Channel<TEvent> receivingChannel;
        private readonly List<Channel<TEvent>> offeringChannels = new List<Channel<TEvent>>();

        public RefCountedBoundedStrategy(int bufferCapacity = 1024)
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

            receivingChannel = Channel.CreateBounded<TEvent>(receivingOptions);

            DistributeEventsAsync();
        }

        public IAsyncEnumerable<TEvent> GetEvents([EnumeratorCancellation] CancellationToken token = default)
        {
            var channel = Channel.CreateBounded<TEvent>(offeringOptions);
            offeringChannels.Add(channel);
            return channel.Reader.ReadAllAsync(token);
        }

        public void Enqueue(TEvent @event)
        {
            EnqueueAsync(receivingChannel, @event);
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
    }
}
