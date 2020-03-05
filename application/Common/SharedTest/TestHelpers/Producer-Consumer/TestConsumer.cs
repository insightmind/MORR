using MORR.Shared.Events.Queue.Strategy;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SharedTest.TestHelpers.Result;

namespace SharedTest.Events.Queue.Strategy
{
    /*
     * The TestConsumer class provides mock for a simple consumer which allows several validation
     * methods on consuming an event. Therefore the Consumer only consumes TestEvents.
     */
    public class TestConsumer
    {
        private readonly IEventQueueStorageStrategy<TestEvent> strategy;
        private Task task;

        /// <summary>
        /// Instantiates a new TestConsumer which should interact with the given strategy.
        /// </summary>
        /// <param name="strategy">The strategy to interact with while consuming.</param>
        public TestConsumer(IEventQueueStorageStrategy<TestEvent> strategy)
        {
            this.strategy = strategy;
        }

        /// <summary>
        /// Starts consuming from the strategy.
        /// </summary>
        /// <param name="runsAsync">Defines whether the producing action should run asynchronously ('true') or synchronously ('false').</param>
        /// <param name="continueCondition">The condition action for defining a producing completion.</param>
        /// <param name="completionAction">The completion action is called on completion. It is also called if any exception occurred.</param>
        public void Consume(bool runsAsync, Func<TestEvent, int, bool> continueCondition, Action<ITestResult> completionAction)
        {
            Debug.Assert(continueCondition != null);
            Debug.Assert(completionAction != null);
            Debug.Assert(strategy != null); 
            
            task = new Task(async () =>
            {
                var count = 0;
                var result = new TestResult();

                try
                {
                    await foreach (var @event in strategy.GetEvents())
                    {
                        count++;
                        if (!continueCondition.Invoke(@event, count))
                        {
                            break;
                        }
                    }

                    result.Complete();
                }
                catch (ChannelConsumingException exception)
                {
                    result.Fail(exception);
                }

                completionAction(result);
            });

            /*
             * This defines the way the consumer is actually run. In minor cases
             * you may want to run it synchronously on the current scheduler
             * e.g. if you want to dequeue all events which were previously committed by a
             * synchronous producer.
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
        /// Runs the Consumer unconditionally as long as the queue does not cancel the consuming itself
        /// through closing the event channel.
        /// </summary>
        /// <param name="runsAsync">Defines whether the producing action should run asynchronously ('true') or synchronously ('false').</param>
        /// <returns>A ManualResetEvent defining whether the consumer was cancelled using the event channel closing.</returns>
        public ManualResetEvent ConsumeUnconditionally(bool runsAsync = true)
        {
            var resetEvent = new ManualResetEvent(false);
            Consume(runsAsync, (@event, num) => true, result => result.EventSuccess(resetEvent));
            return resetEvent;
        }
    }
}
