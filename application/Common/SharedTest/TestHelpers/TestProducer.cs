using MORR.Shared.Events.Queue.Strategy;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedTest.Events.Queue.Strategy
{
    /*
     * The TestProducer class provides a mock for a simple producer of TestEvents which allows several validation
     * methods on producing an event.
     */
    public class TestProducer
    {
        private readonly IEventQueueStorageStrategy<TestEvent> strategy;
        private Task task;

        /// <summary>
        /// Instantiates a new TestProducer for the given EventQueueStorageStrategy.
        /// </summary>
        /// <param name="strategy">The strategy to be interacted with by this producer.</param>
        public TestProducer(IEventQueueStorageStrategy<TestEvent> strategy)
        {
            this.strategy = strategy;
        }

        /// <summary>
        /// Starts a async producing operation which allows to set a condition on
        /// how many events are queued.
        /// </summary>
        /// <param name="continueCondition">The condition action for defining a producing completion.</param>
        /// <returns>Returns a ManualResetEvent which can be used to wait for the process to finish.</returns>
        public ManualResetEvent ProduceAsync(Func<int, bool> continueCondition)
        {
            Debug.Assert(continueCondition != null);
            Debug.Assert(strategy != null);
            var manualResetEvent = new ManualResetEvent(false);

            task = new Task(() =>
            {
                for (var count = 0; continueCondition.Invoke(count); count++)
                {
                    strategy.Enqueue(new TestEvent(count));
                }

                manualResetEvent.Set();
            });

            task.Start();
            return manualResetEvent;
        }
    }
}
