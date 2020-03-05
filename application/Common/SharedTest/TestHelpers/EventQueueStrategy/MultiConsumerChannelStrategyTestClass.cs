using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;
using SharedTest.Events;
using SharedTest.Events.Queue.Strategy;

namespace SharedTest.TestHelpers.EventQueueStrategy
{
    public static class MultiConsumerChannelStrategyTestClass
    {
        private const int maxWaitTime = 500;

        public static void Assert_MaxConsumerReached(MultiConsumerChannelStrategy<TestEvent> strategy, int maxConsumer)
        {
            /* PRECONDITION */
            Debug.Assert(strategy != null);

            var allowedConsumerDidNotFailed = new ManualResetEvent(true);
            var invalidConsumerFailed = new ManualResetEvent(false);

            /* GIVEN */
            for (var index = 0; index < maxConsumer; index++)
            {
                var consumer = new TestConsumer(strategy);
                consumer.Consume(
                    false,
                    (@event, index) => true,
                    result => result?.EventSuccess(allowedConsumerDidNotFailed));
            }

            /* WHEN */
            var invalidConsumer = new TestConsumer(strategy);
            invalidConsumer.Consume(
                true,
                (@event, num) => true,
                result => result?.EventThrows<ChannelConsumingException>(invalidConsumerFailed));


            /* THEN */
            Assert.IsTrue(allowedConsumerDidNotFailed.WaitOne(maxWaitTime), "An Error occurred while consuming using valid consumers.");
            Assert.IsTrue(invalidConsumerFailed.WaitOne(maxWaitTime), "InvalidConsumer should fail.");
        }

        public static void Assert_ConsumerFreed(MultiConsumerChannelStrategy<TestEvent> strategy, int maxConsumer)
        {
            /* PRECONDITION */
            Debug.Assert(strategy != null);
            Debug.Assert(strategy.IsClosed);

            var allowedConsumerDidFail = new ManualResetEvent(false);
            var retryConsumerDidFail = new ManualResetEvent(false);
            var validationConsumer = new TestConsumer(strategy);
            var shouldContinue = true;

            strategy.Open();

            for (var index = 0; index < maxConsumer - 1; index++)
            {
                var consumer = new TestConsumer(strategy);
                _ = consumer.ConsumeUnconditionally();
            }

            /* GIVEN */
            validationConsumer.Consume(
                true,
                (@event, index) => shouldContinue,
                result => result.EventThrows<ChannelConsumingException>(allowedConsumerDidFail));

            /* WHEN */
            var producer = new TestProducer(strategy);
            producer.ProduceUnconditionally();

            shouldContinue = false;

            // We wait until the current consumer exits. After that we can try to retry consuming which should not cause an error.
            Assert.IsFalse(allowedConsumerDidFail.WaitOne(maxWaitTime));

            var newConsumer = new TestConsumer(strategy);
            newConsumer.Consume(true,(@event, num) => true, result => result.EventThrows<ChannelConsumingException>(retryConsumerDidFail));

            /* THEN */
            Assert.IsFalse(allowedConsumerDidFail.WaitOne(maxWaitTime), "An Error occurred while consuming using valid consumers.");
            Assert.IsFalse(retryConsumerDidFail.WaitOne(maxWaitTime), "The retry consumer did unexpectedly fail with an exception.");
        }
    }
}
