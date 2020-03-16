using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Shared.Events.Queue;
using MORR.Shared.Events.Queue.Strategy;
using SharedTest.TestHelpers.Event;

namespace SharedTest.Events.Queue
{
    [TestClass]
    public class EventQueueTest
    {
        public class EventQueueImp : EventQueue<TestEvent>
        {
            public EventQueueImp(IEventQueueStorageStrategy<TestEvent> storageStrategy) : base(storageStrategy) { }
        }

        public Mock<IEventQueueStorageStrategy<TestEvent>> mockStrategy;
        private const int maxWaitTime = 500;
        private EventQueue<TestEvent> queue;

        [TestInitialize]
        public void BeforeTest()
        {
            mockStrategy = new Mock<IEventQueueStorageStrategy<TestEvent>>();
            queue = new EventQueueImp(mockStrategy.Object);
        }

        [TestMethod]
        public void TestEventQueue_IsClosedPropagation()
        {
            /* PRECONDITIONS */
            Debug.Assert(mockStrategy != null);
            Debug.Assert(queue != null);

            /* GIVEN */
            const bool firstValue = true;
            const bool secondValue = false;

            mockStrategy
                .SetupSequence(mock => mock.IsClosed)?
                .Returns(firstValue)?
                .Returns(secondValue);

            /* WHEN */
            var firstIsClosed = queue.IsClosed;
            var secondIsClosed = queue.IsClosed;

            /* THEN */
            Assert.AreEqual(firstValue, firstIsClosed);
            Assert.AreEqual(secondValue, secondIsClosed);

            mockStrategy.VerifyGet(mock => mock.IsClosed, Times.Exactly(2));
        }

        [TestMethod]
        public void TestEventQueue_CloseCalled()
        {
            /* PRECONDITIONS */
            Debug.Assert(mockStrategy != null);
            Debug.Assert(queue != null);

            /* WHEN */
            queue.Close();

            /* THEN */
            mockStrategy.Verify(mock => mock.Close(), Times.Once);
        }

        [TestMethod]
        public void TestEventQueue_OpenCalled()
        {
            /* PRECONDITIONS */
            Debug.Assert(mockStrategy != null);
            Debug.Assert(queue != null);

            /* WHEN */
            queue.Open();

            /* THEN */
            mockStrategy.Verify(mock => mock.Open(), Times.Once);
        }

        [TestMethod]
        public void TestEventQueue_EnqueueCalled()
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
                queue.Enqueue(@event);
            }

            /* THEN */
            mockStrategy.Verify(mock => mock.Enqueue(It.IsIn(enqueueEvents)), Times.Exactly(count));
        }

        [TestMethod]
        public void TestEventQueue_GetEventsCalled()
        {
            /* PRECONDITIONS */
            Debug.Assert(mockStrategy != null);
            Debug.Assert(queue != null);

            /* GIVEN */
            var autoResetEvent = new AutoResetEvent(false);
            var outputEvent = Enumerable.Repeat(new TestEvent(), 50).ToArray();
            var num = 0;

            async IAsyncEnumerable<TestEvent> CallEvent()
            {
                foreach (var innerEvent in outputEvent)
                {
                    yield return innerEvent;
                }
            }

            mockStrategy
                .Setup(mock => mock.GetEvents(It.IsAny<CancellationToken>()))?
                .Returns(CallEvent);

            /* WHEN */
            async void AwaitEach()
            {
                await foreach (var @event in queue.GetEvents())
                {
                    Assert.AreSame(outputEvent[num], @event);
                    num++;
                }

                autoResetEvent.Set();
            }

            AwaitEach();

            /* THEN */
            Assert.IsTrue(autoResetEvent.WaitOne(maxWaitTime));
            mockStrategy.Verify(mock => mock.GetEvents(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
