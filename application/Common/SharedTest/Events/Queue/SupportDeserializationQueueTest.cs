using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Shared.Events.Queue;
using MORR.Shared.Events.Queue.Strategy;
using System.Diagnostics;
using System.Linq;
using SharedTest.TestHelpers.Event;

namespace SharedTest.Events.Queue
{

    [TestClass]
    public class SupportDeserializationEventQueueTest
    {
        public class SupportDeserializationEventQueueImp : SupportDeserializationEventQueue<TestEvent>
        {
            public SupportDeserializationEventQueueImp(IEventQueueStorageStrategy<TestEvent> storageStrategy) : base(storageStrategy) { }
        }

        public Mock<IEventQueueStorageStrategy<TestEvent>> mockStrategy;
        private SupportDeserializationEventQueue<TestEvent> queue;

        [TestInitialize]
        public void BeforeTest()
        {
            mockStrategy = new Mock<IEventQueueStorageStrategy<TestEvent>>();
            queue = new SupportDeserializationEventQueueImp(mockStrategy.Object);
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
