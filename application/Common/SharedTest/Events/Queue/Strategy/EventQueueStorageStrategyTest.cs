using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;
using SharedTest.TestHelpers.EventQueueStrategy;

namespace SharedTest.Events.Queue.Strategy
{
    public abstract class EventQueueStorageStrategyTest<T> where T : IEventQueueStorageStrategy<TestEvent>
    {
        protected T Strategy;

        [TestMethod]
        public void TestBoundedMultiConsumer_OpenSuccess() => EventQueueStorageStrategyTestClass.Assert_OpenSuccess(Strategy);

        [TestMethod]
        public void TestBoundedMultiConsumer_OpenMultiple() => EventQueueStorageStrategyTestClass.Assert_MultipleOpen(Strategy);

        [TestMethod]
        public void TestBoundedMultiConsumer_CloseSuccess() => EventQueueStorageStrategyTestClass.Assert_CloseSuccess(Strategy);

        [TestMethod]
        public void TestBoundedMultiConsumer_CloseMultiple() => EventQueueStorageStrategyTestClass.Assert_MultipleClose(Strategy);
    }
}
