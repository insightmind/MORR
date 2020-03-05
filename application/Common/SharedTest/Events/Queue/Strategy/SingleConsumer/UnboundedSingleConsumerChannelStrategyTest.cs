using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy.SingleConsumer;
using SharedTest.TestHelpers.EventQueueStrategy;

namespace SharedTest.Events.Queue.Strategy.SingleConsumer
{
    [TestClass]
    public class UnboundedSingleConsumerChannelStrategyTest: EventQueueStorageStrategyTest<UnboundedSingleConsumerChannelStrategy<TestEvent>>
    {
        [TestInitialize]
        public void BeforeTest()
        {
            Strategy = new UnboundedSingleConsumerChannelStrategy<TestEvent>();
        }

        [TestMethod]
        public void TestUnboundedSingleConsumer_Enqueue() => EventQueueStorageStrategyTestClass.Assert_EnqueueSingleProducerUnbounded(Strategy);
    }
}
