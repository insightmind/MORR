using System;
using System.Diagnostics;
using System.Threading;
using MORR.Shared.Events.Queue.Strategy;
using SharedTest.TestHelpers.Event;
using SharedTest.TestHelpers.Result;

namespace SharedTest.TestHelpers
{
    /*
     * The TestProducer class provides a mock for a simple producer of TestEvents which allows several validation
     * methods on producing an event.
     */
    public class TestProducer
    {
        private readonly IEventQueueStorageStrategy<TestEvent> strategy;
        private Thread thread;

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
        /// <param name="completionAction">The completion action is called on completion. It is also called if any exception occurred.</param>
        /// <returns>Returns a ManualResetEvent which can be used to wait for the process to finish.</returns>
        public void Produce(Func<int, bool> continueCondition, Action<ITestResult> completionAction)
        {
            Debug.Assert(continueCondition != null);
            Debug.Assert(strategy != null);

            thread = new Thread(() =>
            {
                var result = new TestResult();

                try
                {
                    for (var count = 0; continueCondition.Invoke(count); count++)
                    {
                        strategy.Enqueue(new TestEvent(count));
                    }

                    result.Complete();
                }
                catch (Exception exception)
                {
                    result.Fail(exception);
                }

                completionAction(result);
            });

            thread.Start();
        }

        /// <summary>
        /// Produces new TestEvents unconditionally.
        /// </summary>
        public void ProduceUnconditionally()
        {
            Produce(num => true, _ => { });
        }
    }
}
