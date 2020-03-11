using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;
using SharedTest.Events;
using SharedTest.Events.Queue.Strategy;

namespace SharedTest.TestHelpers.EventQueueStrategy
{
    public static class UnboundedConsumerChannelStrategyTestClass
    {
        private const int maxWaitTime = 500;
        private const int maxEvents = 100;

        public static void Assert_EnqueueSingleProducerUnbounded(IEventQueueStorageStrategy<TestEvent> strategy)
        {
            /* PRECONDITION */
            Debug.Assert(strategy != null);
            Debug.Assert(strategy.IsClosed);

            /* GIVEN */
            strategy.Open();
            var producer = new TestProducer(strategy);
            var consumer = new TestConsumer(strategy);
            var produceEvent = new ManualResetEvent(false);
            var consumeEvent = new ManualResetEvent(false);

            /* WHEN */
            consumer.Consume(false, (@event, num) => num < maxEvents, result => result.EventSuccess(consumeEvent));
            producer.Produce(false, num => num < maxEvents, result => result.EventSuccess(produceEvent));

            /* THEN */
            Assert.IsTrue(produceEvent.WaitOne(maxWaitTime), "Producer did not finish in time");
            Assert.IsTrue(consumeEvent.WaitOne(maxWaitTime), "Consumer did not receive events in time");
        }
    }
}
