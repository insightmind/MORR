using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;
using SharedTest.Events;
using SharedTest.Events.Queue.Strategy;

namespace SharedTest.TestHelpers.EventQueueStrategy
{
    public static class EventQueueStorageStrategyTestClass
    {
        private const int maxWaitTime = 500;
        private const int maxEvents = 100;

        public static void Assert_OpenSuccess(IEventQueueStorageStrategy<TestEvent> strategy)
        {
            /* PRECONDITION */
            Debug.Assert(strategy != null);
            Debug.Assert(strategy.IsClosed);

            /* WHEN */
            strategy.Open();

            /* THEN */
            Assert.IsFalse(strategy.IsClosed);
        }

        public static void Assert_MultipleOpen(IEventQueueStorageStrategy<TestEvent> strategy)
        {
            /* PRECONDITION */
            Debug.Assert(strategy != null);
            Debug.Assert(strategy.IsClosed);

            /* WHEN */
            strategy.Open();
            strategy.Open();

            /* THEN */
            Assert.IsFalse(strategy.IsClosed);
        }

        public static void Assert_CloseSuccess(IEventQueueStorageStrategy<TestEvent> strategy)
        {
            /* PRECONDITION */
            Debug.Assert(strategy != null);
            Debug.Assert(strategy.IsClosed);

            /* GIVEN */
            strategy.Open();
            Assert.IsFalse(strategy.IsClosed);

            /* WHEN */
            strategy.Close();

            /* THEN */
            Assert.IsTrue(strategy.IsClosed);
        }

        public static void Assert_MultipleClose(IEventQueueStorageStrategy<TestEvent> strategy)
        {
            /* PRECONDITION */
            Debug.Assert(strategy != null);
            Debug.Assert(strategy.IsClosed);

            /* GIVEN */
            strategy.Open();
            Assert.IsFalse(strategy.IsClosed);

            /* WHEN */
            strategy.Close();
            strategy.Close();

            /* THEN */
            Assert.IsTrue(strategy.IsClosed);
        }

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
            consumer.Consume(true, (@event, num) => num < maxEvents, result => result.EventSuccess(consumeEvent));
            producer.Produce(true, num => num < maxEvents, result => result.EventSuccess(produceEvent));

            /* THEN */
            Assert.IsTrue(produceEvent.WaitOne(maxWaitTime));
            Assert.IsTrue(consumeEvent.WaitOne(maxWaitTime));
        }
    }
}
