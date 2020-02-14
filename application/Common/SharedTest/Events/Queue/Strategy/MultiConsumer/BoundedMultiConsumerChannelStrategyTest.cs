using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace SharedTest.Events.Queue.Strategy.MultiConsumer
{
    [TestClass]
    public class BoundedMultiConsumerChannelStrategyTest
    {
        const uint defaultMaxConsumer = 2;
        const int defaultMaxEvents = 2;
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
            Assert.ThrowsException<ChannelConsumingException>(() => new BoundedMultiConsumerChannelStrategy<TestEvent>(maxEvents, maxConsumers));

            /* THEN */
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

            /* GIVEN */

            /* WHEN */

            /* THEN */
        }

        [TestMethod]
        public void TestBoundedMultiConsumer_MaxEventBoundReached()
        {
            /* PRECONDITION */
            Debug.Assert(strategy != null);

            /* GIVEN */

            /* WHEN */

            /* THEN */
        }
    }
}
