using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MORR.Shared.Events.Queue.Strategy
{
    public class KeepAllStorageStrategy<TEvent>: IEventQueueStorageStrategy<TEvent> where TEvent: Event
    {
        private Channel<TEvent> eventChannel = Channel.CreateUnbounded<TEvent>(
            new UnboundedChannelOptions() { 
                AllowSynchronousContinuations = true,
                SingleReader = false,
                SingleWriter = false
            }); 

        public IAsyncEnumerable<TEvent> GetEvents([EnumeratorCancellation] CancellationToken token = default)
        {
            return eventChannel.Reader.ReadAllAsync(token);
        }

        public void Enqueue(TEvent @event)
        {
            EnqueueAsync(@event);
        }

        private ValueTask EnqueueAsync(TEvent @event)
        {
            async Task AsyncSlowPath(TEvent @event)
            {
                await eventChannel.Writer.WriteAsync(@event);
            }

            return eventChannel.Writer.TryWrite(@event) ? default : new ValueTask(AsyncSlowPath(@event));
        }
    }
}
