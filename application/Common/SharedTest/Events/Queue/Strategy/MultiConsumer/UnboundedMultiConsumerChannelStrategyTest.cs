using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace SharedTest.Events.Queue.Strategy.MultiConsumer
{
    [TestClass]
    class UnboundedMultiConsumerChannelStrategyTest
    {
        [TestMethod]
        public void TestBoundedMultiConsumer_MaxSingleConsumer()
        {
            /* GIVEN */
            const uint maxConsumers = 1;

            /* WHEN */
            Assert.ThrowsException<ChannelConsumingException>(() => new UnboundedMultiConsumerChannelStrategy<TestEvent>(maxConsumers));

            /* THEN */
        }

        [TestMethod]
        public void TestBoundedMultiConsumer_NoConsumer()
        {
            /* GIVEN */
            const uint maxConsumers = 0;

            /* WHEN */
            Assert.ThrowsException<ChannelConsumingException>(() => new UnboundedMultiConsumerChannelStrategy<TestEvent>(maxConsumers));
        }
    }
}
