using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace SharedTest.Events.Queue.Strategy.MultiConsumer
{
    [TestClass]
    public class BoundedMultiConsumerChannelStrategyTest
    {
        private const uint defaultMaxConsumer = 2;
        private const int defaultMaxEvents = 2;
        protected const int maxWaitTime = 500;
        private BoundedMultiConsumerChannelStrategy<TestEvent> strategy;

        [TestInitialize]
        public void BeforeTest()
        {
            strategy = new BoundedMultiConsumerChannelStrategy<TestEvent>(defaultMaxEvents, defaultMaxConsumer);
        }

        [TestMethod]
        public void TestBoundedMultiConsumer_MaxSingleConsumer()
        {
            /* GIVEN */
            const uint maxConsumers = 1;
            const int maxEvents = 10;

            /* WHEN */
            // This should throw as a maximum of 1 consumer is respectively not allowed with a multi consumer strategy.
            // In this case the programmer should use the BoundedSingleConsumerChannelStrategy.
            Assert.ThrowsException<ChannelConsumingException>(() => new BoundedMultiConsumerChannelStrategy<TestEvent>(maxEvents, maxConsumers));
        }

        [TestMethod]
        public void TestBoundedMultiConsumer_NoConsumer()
        {
            /* GIVEN */
            const uint maxConsumers = 0;
            const int maxEvents = 10;

            /* WHEN */
            Assert.ThrowsException<ChannelConsumingException>(() => new BoundedMultiConsumerChannelStrategy<TestEvent>(maxEvents, maxConsumers));
        }

        [TestMethod]
        public void TestBoundedMultiConsumer_MaxConsumerReached()
        {
            /* PRECONDITION */
            Debug.Assert(strategy != null);

            var allowedConsumerDidNotFailed = new ManualResetEvent(true);
            var invalidConsumerFailed = new ManualResetEvent(false);

            /* GIVEN */
            strategy.Open();

            for (var index = 0; index < defaultMaxConsumer; index++)
            {
                var consumer = new TestConsumer(strategy);
                consumer.Consume(
                    false,
                    (@event, index) => true,
                    result => result?.WasSuccess(allowedConsumerDidNotFailed));
            }

            /* WHEN */
            var invalidConsumer = new TestConsumer(strategy);
            invalidConsumer.Consume(
                true,
                (@event, num) => true,
                result => result?.DidFailThrowing<ChannelConsumingException>(invalidConsumerFailed));
                

            /* THEN */
            Assert.IsTrue(allowedConsumerDidNotFailed.WaitOne(maxWaitTime), "An Error occurred while consuming using valid consumers.");
            Assert.IsTrue(invalidConsumerFailed.WaitOne(maxWaitTime), "InvalidConsumer should fail.");
        }

        [TestMethod]
        public void TestBoundedMultiConsumer_MaxEventBoundReached()
        {
            /* PRECONDITION */
            Debug.Assert(strategy != null);

            /* GIVEN */
            strategy.Open();

            /* WHEN */
            var producer = new TestProducer(strategy);

            /* THEN */
        }
    }
}
