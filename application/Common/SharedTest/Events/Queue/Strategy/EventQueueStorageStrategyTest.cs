using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;
using SharedTest.TestHelpers.Event;

namespace SharedTest.Events.Queue.Strategy
{
    public abstract class EventQueueStorageStrategyTest<T> where T : IEventQueueStorageStrategy<TestEvent>
    {
        protected T Strategy;

        [TestMethod]
        public void TestEventQueueStorageStrategy_OpenSuccess()
        {
            /* PRECONDITION */
            Debug.Assert(Strategy != null);
            Debug.Assert(Strategy.IsClosed);

            /* WHEN */
            Strategy.Open();

            /* THEN */
            Assert.IsFalse(Strategy.IsClosed);
        }

        [TestMethod]
        public void TestEventQueueStorageStrategy_OpenMultiple()
        {
            /* PRECONDITION */
            Debug.Assert(Strategy != null);
            Debug.Assert(Strategy.IsClosed);

            /* WHEN */
            Strategy.Open();
            Strategy.Open();

            /* THEN */
            Assert.IsFalse(Strategy.IsClosed);
        }

        [TestMethod]
        public void TestEventQueueStorageStrategy_CloseSuccess()
        {
            /* PRECONDITION */
            Debug.Assert(Strategy != null);
            Debug.Assert(Strategy.IsClosed);

            /* GIVEN */
            Strategy.Open();
            Assert.IsFalse(Strategy.IsClosed);

            /* WHEN */
            Strategy.Close();

            /* THEN */
            Assert.IsTrue(Strategy.IsClosed);
        }

        [TestMethod]
        public void TestEventQueueStorageStrategy_CloseMultiple()
        {
            /* PRECONDITION */
            Debug.Assert(Strategy != null);
            Debug.Assert(Strategy.IsClosed);

            /* GIVEN */
            Strategy.Open();
            Assert.IsFalse(Strategy.IsClosed);

            /* WHEN */
            Strategy.Close();
            Strategy.Close();

            /* THEN */
            Assert.IsTrue(Strategy.IsClosed);
        }
    }
}
