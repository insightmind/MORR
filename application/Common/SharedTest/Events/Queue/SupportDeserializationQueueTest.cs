using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Shared.Events.Queue;
using MORR.Shared.Events.Queue.Strategy;
using System.Diagnostics;
using System.Linq;
using MORR.Shared.Events;
using SharedTest.TestHelpers.EventQueue;

namespace SharedTest.Events.Queue
{

    [TestClass]
    public class SupportDeserializationEventQueueTest : EventQueueTestClass<SupportDeserializationEventQueue<TestEvent>>
    {
        public class EventQueueImp : SupportDeserializationEventQueue<TestEvent>
        {
            public EventQueueImp(IEventQueueStorageStrategy<TestEvent> storageStrategy) : base(storageStrategy) { }
        }

        private SupportDeserializationEventQueue<TestEvent> queue;

        [TestInitialize]
        public override void BeforeTest()
        {
            base.BeforeTest();
            queue = new EventQueueImp(mockStrategy.Object);
        }

        [TestMethod]
        public void TestSupportDeserializationEventQueue_EnqueueUntyped()
        {
            /* PRECONDITIONS */
            Debug.Assert(mockStrategy != null);
            Debug.Assert(queue != null);

            /* GIVEN */
            const int count = 50;
            var enqueueEvents = Enumerable.Repeat(new TestEvent(), count).ToArray();

            /* WHEN */
            foreach (var @event in enqueueEvents)
            {
                queue.Enqueue((object) @event);
            }

            /* THEN */
            mockStrategy.Verify(mock => mock.Enqueue(It.IsIn(enqueueEvents)), Times.Exactly(count));
        }

        [TestMethod]
        public void TestSupportDeserializationEventQueue_EnqueueUntypedInvalidType()
        {
            /* PRECONDITIONS */
            Debug.Assert(mockStrategy != null);
            Debug.Assert(queue != null);

            /* GIVEN */
            const int count = 50;
            var enqueueEvents = Enumerable.Repeat(new InvalidTypeTestEvent(), count).ToArray();

            /* WHEN */
            foreach (var @event in enqueueEvents)
            {
                // More or less already: THEN
                Assert.ThrowsException<ArgumentException>(() => queue.Enqueue((object) @event));
            }

            /* THEN */
            mockStrategy.Verify(mock => mock.Enqueue(It.IsAny<TestEvent>()), Times.Never);
        }
    }
}
