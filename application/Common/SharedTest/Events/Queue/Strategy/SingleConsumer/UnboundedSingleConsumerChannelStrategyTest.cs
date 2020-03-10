using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy.SingleConsumer;

namespace SharedTest.Events.Queue.Strategy.SingleConsumer
{
    [TestClass]
    public class UnboundedSingleConsumerChannelStrategyTest: SingleConsumerChannelStrategyTest<UnboundedSingleConsumerChannelStrategy<TestEvent>>
    {
        [TestInitialize]
        public void BeforeTest()
        {
            Strategy = new UnboundedSingleConsumerChannelStrategy<TestEvent>();
        }
    }
}
