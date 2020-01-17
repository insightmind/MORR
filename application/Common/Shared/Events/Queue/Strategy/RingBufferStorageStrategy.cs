using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MORR.Shared.Events.Queue.Strategy
{
    public class RingBufferStorageStrategy<TEvent> : IEventQueueStorageStrategy<TEvent> where TEvent : Event
    {
        public IAsyncEnumerable<TEvent> GetEvents([EnumeratorCancellation] CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Enqueue(TEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
