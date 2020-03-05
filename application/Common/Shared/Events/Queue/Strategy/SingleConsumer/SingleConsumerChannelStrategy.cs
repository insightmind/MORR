using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MORR.Shared.Events.Queue.Strategy.SingleConsumer
{
    /// <summary>
    /// A SingleConsumerChannelStrategy is a Distributive-FIFO Queue which allows multiple producers and a single consumer.
    /// This queue is performance optimized for a single consumer and should be preferred in this case.
    /// Please make sure only a single consumer will request. Otherwise an exception is thrown.
    /// </summary>
    /// <typeparam name="TEvent">The type of event which is queued by the channel</typeparam>
    public abstract class SingleConsumerChannelStrategy<TEvent> : IEventQueueStorageStrategy<TEvent> where TEvent : Event
    {
        private Channel<TEvent> eventChannel;
        private bool isOccupied;
        private readonly Mutex subscriptionMutex = new Mutex();
        private const int timeOut = 500;
        public bool IsClosed { get; private set; } = true;

        /// <summary>
        ///     Asynchronously gets all events as concrete type <typeparamref name="T" />.
        /// </summary>
        /// <returns>A stream of <typeparamref name="T" /></returns>
        public IAsyncEnumerable<TEvent> GetEvents(CancellationToken token = default)
        {
            subscriptionMutex.WaitOne(timeOut);

            if (isOccupied)
            {
                subscriptionMutex.ReleaseMutex();
                throw new ChannelConsumingException("Channel already occupied!");
            }

            isOccupied = true;
            token.Register(FreeReading);
            
            subscriptionMutex.ReleaseMutex();
            return eventChannel.Reader.ReadAllAsync(token);
        }

        /// <summary>
        ///     Asynchronously enqueues a new event.
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        public async void Enqueue(TEvent @event)
        {
            await EnqueueAsync(@event);
        }

        public void Open()
        {
            if (!IsClosed) return;
            FreeReading();
            
            eventChannel = CreateChannel();
            IsClosed = false;
        }

        public void Close()
        {
            if (IsClosed) return;

            IsClosed = true;
            eventChannel.Writer.Complete();
            FreeReading();
        }

        private ValueTask EnqueueAsync(TEvent @event)
        {
            async Task AsyncSlowPath(TEvent @event) => await eventChannel.Writer.WriteAsync(@event);
            return eventChannel.Writer.TryWrite(@event) ? default : new ValueTask(AsyncSlowPath(@event));
        }

        private void FreeReading()
        {
            subscriptionMutex.WaitOne(timeOut);
            isOccupied = false;
            subscriptionMutex.ReleaseMutex();
        }

        protected abstract Channel<TEvent> CreateChannel();
    }
}
