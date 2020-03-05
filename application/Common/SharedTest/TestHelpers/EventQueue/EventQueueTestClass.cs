using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Shared.Events.Queue;
using MORR.Shared.Events.Queue.Strategy;
using SharedTest.Events;

namespace SharedTest.TestHelpers.EventQueue
{
    public abstract class EventQueueTestClass<TQueue> where TQueue : EventQueue<TestEvent>
    {
        public Mock<IEventQueueStorageStrategy<TestEvent>> mockStrategy;

        protected const int maxWaitTime = 500;

        public virtual void BeforeTest()
        {
            mockStrategy = new Mock<IEventQueueStorageStrategy<TestEvent>>();
        }

        public void Assert_IsClosedPropagation(TQueue queue)
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

        public void Assert_CloseCalled(TQueue queue)
        {
            /* PRECONDITIONS */
            Debug.Assert(mockStrategy != null);
            Debug.Assert(queue != null);

            /* WHEN */
            queue.Close();

            /* THEN */
            mockStrategy.Verify(mock => mock.Close(), Times.Once);
        }

        public void Assert_OpenCalled(TQueue queue)
        {
            /* PRECONDITIONS */
            Debug.Assert(mockStrategy != null);
            Debug.Assert(queue != null);

            /* WHEN */
            queue.Open();

            /* THEN */
            mockStrategy.Verify(mock => mock.Open(), Times.Once);
        }

        public void Assert_EnqueueCalled(TQueue queue)
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

        public void Assert_GetEventsCalled(TQueue queue)
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
