using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue;
using MORR.Shared.Events.Queue.Strategy;

namespace SharedTest.Events.Queue
{
    [TestClass]
    public class EventQueueTest : EventQueueTestClass<EventQueue<TestEvent>>
    {
        public class EventQueueImp : EventQueue<TestEvent>
        {
            public EventQueueImp(IEventQueueStorageStrategy<TestEvent> storageStrategy) : base(storageStrategy) { }
        }

        private EventQueue<TestEvent> queue;

        [TestInitialize]
        public override void BeforeTest()
        {
            base.BeforeTest();
            queue = new EventQueueImp(mockStrategy.Object);
        }

        [TestMethod]
        public void TestEventQueue_IsClosedPropagation() => Assert_IsClosedPropagation(queue);

        [TestMethod]
        public void TestEventQueue_CloseCalled() => Assert_CloseCalled(queue);

        [TestMethod]
        public void TestEventQueue_OpenCalled() => Assert_OpenCalled(queue);

        [TestMethod]
        public void TestEventQueue_EnqueueCalled() => Assert_EnqueueCalled(queue);

        [TestMethod]
        public void TestEventQueue_GetEventsCalled() => Assert_GetEventsCalled(queue);
    }
}
