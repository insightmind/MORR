using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace SharedTest.Events.Queue.Strategy.MultiConsumer
{
    public class MultiConsumerChannelStrategyTestClass
    {
        protected const int maxWaitTime = 500;

        public void Assert_MaxConsumerReached(MultiConsumerChannelStrategy<TestEvent> strategy, int maxConsumer)
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
    }
}
