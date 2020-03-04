using MORR.Shared.Events.Queue.Strategy;
using SharedTest.TestHelpers;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
        /// <param name="runsAsync">Defines whether the producing action should run asynchronously ('true') or synchronously ('false').</param>
        /// <param name="continueCondition">The condition action for defining a producing completion.</param>
        /// <param name="completionAction">The completion action is called on completion. It is also called if any exception occurred.</param>
        /// <returns>Returns a ManualResetEvent which can be used to wait for the process to finish.</returns>
        public void Produce(bool runsAsync, Func<int, bool> continueCondition, Action<TestResult> completionAction)
        {
            Debug.Assert(continueCondition != null);
            Debug.Assert(strategy != null);

            task = new Task(() =>
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

            if (runsAsync)
            {
                task.Start();
            }
            else
            {
                task.RunSynchronously();
            }
        }
    }
}
