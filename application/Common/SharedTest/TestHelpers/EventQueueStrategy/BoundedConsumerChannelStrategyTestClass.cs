using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;
using SharedTest.Events;
using SharedTest.Events.Queue.Strategy;

namespace SharedTest.TestHelpers.EventQueueStrategy
{
    public static class BoundedConsumerChannelStrategyTestClass
    {
        private const int maxWaitTime = 500;
        private const int maxEvents = 100;

        public static void Assert_EnqueueSingleProducerBounded(IEventQueueStorageStrategy<TestEvent> strategy, int maxEventCount)
        {
            /* PRECONDITION */
            Debug.Assert(strategy != null);
            Debug.Assert(maxEventCount >= 0);
            Debug.Assert(strategy.IsClosed);

            /* GIVEN */
            var expectedEventCount = (maxEventCount < maxEvents) ? maxEventCount : maxEvents;
            var producer = new TestProducer(strategy);
            var consumer = new TestConsumer(strategy);
            var produceEvent = new ManualResetEvent(false);
            var consumeEvent = new ManualResetEvent(false);

            /* WHEN */
            strategy.Open();
            Assert.IsTrue(!strategy.IsClosed);

            consumer.Consume(false, (@event, num) => num < expectedEventCount, result => result.EventSuccess(consumeEvent));
            producer.Produce(false, num => num < maxEvents, result => result.EventSuccess(produceEvent));

            /* THEN */
            Assert.IsTrue(produceEvent.WaitOne(maxWaitTime), "Producer did not complete successfully");
            Assert.IsTrue(consumeEvent.WaitOne(maxWaitTime), "Consumer did not complete successfully");
        }
    }
}
