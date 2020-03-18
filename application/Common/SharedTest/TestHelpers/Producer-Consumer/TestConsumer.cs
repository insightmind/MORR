using System;
using System.Diagnostics;
using System.Threading;
using MORR.Shared.Events.Queue.Strategy;
using SharedTest.TestHelpers.Event;
using SharedTest.TestHelpers.Result;
using SharedTest.TestHelpers.Utility;

namespace SharedTest.TestHelpers
{
    /*
     * The TestConsumer class provides mock for a simple consumer which allows several validation
     * methods on consuming an event. Therefore the Consumer only consumes TestEvents.
     */
    public class TestConsumer
    {
        private readonly IEventQueueStorageStrategy<TestEvent> strategy;
        private Thread thread;

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
        /// <param name="continueCondition">The condition action for defining a producing completion.</param>
        /// <param name="completionAction">The completion action is called on completion. It is also called if any exception occurred.</param>
        public void Consume(Func<TestEvent, int, bool> continueCondition, Action<ITestResult> completionAction, int maxWaitTime = 500)
        {
            Debug.Assert(continueCondition != null);
            Debug.Assert(completionAction != null);
            Debug.Assert(strategy != null);

            using var awaitsThreadEvent = new ManualResetEvent(false);

            thread = new Thread(async () =>
            {
                var count = 0;
                var result = new TestResult();

                try
                {
                    var tokenSource = new CancellationTokenSource();
                    await foreach (var @event in Awaitable.Await(strategy.GetEvents(tokenSource.Token), awaitsThreadEvent))
                    {
                        count++;
                        if (continueCondition.Invoke(@event, count)) continue;

                        tokenSource.Cancel();
                        break;
                    }

                    result.Complete();
                }
                catch (ChannelConsumingException exception)
                {
                    result.Fail(exception);
                }

                completionAction(result);
            });

            thread.Start();
            awaitsThreadEvent.WaitOne(maxWaitTime);
        }

        /// <summary>
        /// Runs the Consumer unconditionally as long as the queue does not cancel the consuming itself
        /// through closing the event channel.
        /// </summary>
        /// <returns>A ManualResetEvent defining whether the consumer was cancelled using the event channel closing.</returns>
        public void ConsumeUnconditionally(int maxWaitTime, Action<ITestResult> completionAction)
        {
            Consume((@event, num) => true, completionAction, maxWaitTime);
        }
    }
}
