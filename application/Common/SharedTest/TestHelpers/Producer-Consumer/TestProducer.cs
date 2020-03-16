using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MORR.Shared.Events.Queue.Strategy;
using SharedTest.Events;
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
        public void Produce(bool runsAsync, Func<int, bool> continueCondition, Action<ITestResult> completionAction)
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

            /*
             * This defines the way the producer is actually run. In minor cases
             * you may want to run it synchronously on the current scheduler
             * e.g. if you want to commit all events prior to testing.
             *
             * Otherwise I encourage you to choose the default
             * asynchronous running. However you may than need to listen to the completion action
             * to gather test information.
             */
            if (runsAsync)
            {
                task.Start();
            }
            else
            {
                task.RunSynchronously();
            }
        }

        /// <summary>
        /// Produces new TestEvents unconditionally.
        /// </summary>
        /// <param name="runsAsync">Defines whether the producing action should run asynchronously ('true') or synchronously ('false').</param>
        public void ProduceUnconditionally(bool runsAsync = true)
        {
            Produce(runsAsync, num => true, _ => { });
        }
    }
}
