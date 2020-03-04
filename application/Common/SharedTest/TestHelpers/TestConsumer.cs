using MORR.Shared.Events.Queue.Strategy;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedTest.Events.Queue.Strategy
{
    public class TestResult
    {
        private bool didComplete;
        private Exception? exception;

        public void Complete()
        {
            didComplete = true;
        }

        public void Fail(Exception exception)
        {
            didComplete = true;
            this.exception = exception;
        }

        public void AssertDidCompleteSuccessfully()
        {
            Assert.IsTrue(didComplete);
            Assert.IsNull(exception);
        }

        public void AssertDidCompleteThrowing<T>() where T : Exception
        {
            Assert.IsTrue(didComplete);
            Assert.IsInstanceOfType(exception, typeof(T));
        }

        public void WasSuccess(ManualResetEvent resetEvent)
        {
            Debug.Assert(resetEvent != null);

            if (didComplete && exception == null)
            {
                resetEvent.Set();
            }
            else
            {
                resetEvent.Reset();
            }
        }

        public void DidFailThrowing<T>(ManualResetEvent resetEvent) where T : Exception
        {
            Debug.Assert(resetEvent != null);

            if (exception?.GetType() == typeof(T))
            {
                resetEvent.Set();
            }
            else
            {
                resetEvent.Reset();
            }
        }
    }

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

        public void Consume(bool runsAsync, Func<TestEvent, int, bool> continueCondition, Action<TestResult> completionAction)
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
