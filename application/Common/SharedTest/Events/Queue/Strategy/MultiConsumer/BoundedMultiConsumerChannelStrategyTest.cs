using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;
using SharedTest.TestHelpers.EventQueueStrategy;

namespace SharedTest.Events.Queue.Strategy.MultiConsumer
{
    [TestClass]
    public class BoundedMultiConsumerChannelStrategyTest: EventQueueStorageStrategyTest<BoundedMultiConsumerChannelStrategy<TestEvent>>
    {
        private const int defaultMaxConsumer = 5;
        private const int defaultMaxEvents = 10;

        [TestInitialize]
        public void BeforeTest()
        {
            Strategy = new BoundedMultiConsumerChannelStrategy<TestEvent>(defaultMaxEvents, defaultMaxConsumer);
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
        public void TestBoundedMultiConsumer_MaxConsumerReached() => MultiConsumerChannelStrategyTestClass.Assert_MaxConsumerReached(Strategy, defaultMaxConsumer);

        [TestMethod]
        public void TestBoundedMultiConsumer_MaxEventBoundReached() => BoundedConsumerChannelStrategyTestClass.Assert_EnqueueSingleProducerBounded(Strategy, defaultMaxEvents);

        [TestMethod]
        public void TestBoundedMultiConsumer_FreeConsumer() => MultiConsumerChannelStrategyTestClass.Assert_ConsumerFreed(Strategy, defaultMaxConsumer);

        [TestMethod]
        public void TestBoundedMultiConsumer_DistributeEvents() => MultiConsumerChannelStrategyTestClass.Assert_DistributeElements(Strategy, defaultMaxConsumer, defaultMaxEvents / 2);
    }
}
