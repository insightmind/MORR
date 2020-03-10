using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy.SingleConsumer;
using SharedTest.TestHelpers.EventQueueStrategy;

namespace SharedTest.Events.Queue.Strategy.SingleConsumer
{
    [TestClass]
    public class BoundedSingleConsumerChannelStrategyTest: EventQueueStorageStrategyTest<BoundedSingleConsumerChannelStrategy<TestEvent>>
    {
        private const int defaultMaxEvents = 2;

        [TestInitialize]
        public void BeforeTest()
        {
            Strategy = new BoundedSingleConsumerChannelStrategy<TestEvent>(defaultMaxEvents);
        }

        [TestMethod]
        public void TestBoundedSingleConsumer_MaxEventBoundReached() => BoundedConsumerChannelStrategyTestClass.Assert_EnqueueSingleProducerBounded(Strategy, defaultMaxEvents);
    }
}
