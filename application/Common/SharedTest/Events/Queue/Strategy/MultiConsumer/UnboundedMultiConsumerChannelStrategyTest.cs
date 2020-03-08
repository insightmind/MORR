using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;
using SharedTest.TestHelpers.EventQueueStrategy;

namespace SharedTest.Events.Queue.Strategy.MultiConsumer
{
    [TestClass]
    public class UnboundedMultiConsumerChannelStrategyTest: EventQueueStorageStrategyTest<UnboundedMultiConsumerChannelStrategy<TestEvent>>
    {
        private const int defaultMaxConsumer = 2;

        [TestInitialize]
        public void BeforeTest()
        {
            Strategy = new UnboundedMultiConsumerChannelStrategy<TestEvent>(defaultMaxConsumer);
        }

        [TestMethod]
        public void TestUnboundedMultiConsumer_MaxSingleConsumer()
        {
            /* GIVEN */
            const uint maxConsumers = 1;

            /* WHEN */
            Assert.ThrowsException<ChannelConsumingException>(() => new UnboundedMultiConsumerChannelStrategy<TestEvent>(maxConsumers));
        }

        [TestMethod]
        public void TestUnboundedMultiConsumer_NoConsumer()
        {
            /* GIVEN */
            const uint maxConsumers = 0;

            /* WHEN */
            Assert.ThrowsException<ChannelConsumingException>(() => new UnboundedMultiConsumerChannelStrategy<TestEvent>(maxConsumers));
        }

        [TestMethod]
        public void TestUnboundedMultiConsumer_MaxConsumerReached() => MultiConsumerChannelStrategyTestClass.Assert_MaxConsumerReached(Strategy, defaultMaxConsumer);

        [TestMethod]
        public void TestUnboundedMultiConsumer_Enqueue() => EventQueueStorageStrategyTestClass.Assert_EnqueueSingleProducerUnbounded(Strategy);

        [TestMethod]
        public void TestUnboundedMultiConsumer_FreeConsumer() => MultiConsumerChannelStrategyTestClass.Assert_ConsumerFreed(Strategy, defaultMaxConsumer);

        [TestMethod]
        public void TestUnboundedMultiConsumer_DistributeEvents() => MultiConsumerChannelStrategyTestClass.Assert_DistributeElements(Strategy, defaultMaxConsumer);
    }
}
