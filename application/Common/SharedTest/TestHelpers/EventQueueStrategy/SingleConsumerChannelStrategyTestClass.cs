using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;
using MORR.Shared.Events.Queue.Strategy.SingleConsumer;
using SharedTest.Events;
using SharedTest.Events.Queue.Strategy;

namespace SharedTest.TestHelpers.EventQueueStrategy
{
    public static class SingleConsumerChannelStrategyTestClass
    {
        private const int maxWaitTime = 500;

        public static void Assert_IsOccupied(SingleConsumerChannelStrategy<TestEvent> strategy)
        {
            /* PRECONDITION */
            Debug.Assert(strategy != null);

            var allowedConsumerDidNotFailed = new ManualResetEvent(true);
            var invalidConsumerFailed = new ManualResetEvent(false);

            /* GIVEN */
            var consumer = new TestConsumer(strategy);
            consumer.Consume(
                false,
                (@event, index) => true,
                result => result?.EventSuccess(allowedConsumerDidNotFailed));

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
    }
}
