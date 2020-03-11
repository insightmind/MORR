using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;

namespace SharedTest.Events.Queue.Strategy.MultiConsumer
{
    public abstract class MultiConsumerChannelStrategyTest<T>: EventQueueStorageStrategyTest<T> where T : IEventQueueStorageStrategy<TestEvent>
    {
        private const int maxWaitTime = 1000;
        protected const int defaultMaxConsumer = 5;
        protected const int defaultMaxEvents = 10;

        [TestMethod]
        public void TestMultiConsumer_MaxConsumerReached()
        {
            /* PRECONDITION */
            Debug.Assert(Strategy != null);

            var allowedConsumerDidNotFailed = new ManualResetEvent(true);
            var invalidConsumerFailed = new ManualResetEvent(false);

            /* GIVEN */
            for (var index = 0; index < defaultMaxConsumer; index++)
            {
                var consumer = new TestConsumer(Strategy);
                consumer.Consume(
                    false,
                    (@event, index) => true,
                    result => result?.EventSuccess(allowedConsumerDidNotFailed));
            }

            /* WHEN */
            var invalidConsumer = new TestConsumer(Strategy);
            invalidConsumer.Consume(
                false,
                (@event, num) => true,
                result => result?.EventThrows<ChannelConsumingException>(invalidConsumerFailed));


            /* THEN */
            Assert.IsTrue(allowedConsumerDidNotFailed.WaitOne(maxWaitTime), "An Error occurred while consuming using valid consumers.");
            Assert.IsTrue(invalidConsumerFailed.WaitOne(maxWaitTime), "InvalidConsumer should fail.");
        }

        [TestMethod]
        public void TestMultiConsumer_FreeConsumer()
        {
            /* PRECONDITION */
            Debug.Assert(Strategy != null);
            Debug.Assert(Strategy.IsClosed);

            var allowedConsumerDidFail = new ManualResetEvent(false);
            var retryConsumerDidFail = new ManualResetEvent(false);
            var validationConsumer = new TestConsumer(Strategy);
            var shouldContinue = true;

            Strategy.Open();

            for (var index = 0; index < defaultMaxConsumer - 1; index++)
            {
                var consumer = new TestConsumer(Strategy);
                _ = consumer.ConsumeUnconditionally();
            }

            /* GIVEN */
            validationConsumer.Consume(
                false,
                (@event, index) => shouldContinue,
                result => result.EventThrows<ChannelConsumingException>(allowedConsumerDidFail));

            /* WHEN */
            var producer = new TestProducer(Strategy);
            producer.ProduceUnconditionally();

            shouldContinue = false;

            // We wait until the current consumer exits. After that we can try to retry consuming which should not cause an error.
            Assert.IsFalse(allowedConsumerDidFail.WaitOne(maxWaitTime));

            var newConsumer = new TestConsumer(Strategy);
            newConsumer.Consume(true,(@event, num) => true, result => result.EventThrows<ChannelConsumingException>(retryConsumerDidFail));

            /* THEN */
            Assert.IsFalse(allowedConsumerDidFail.WaitOne(maxWaitTime), "An Error occurred while consuming using valid consumers.");
            Assert.IsFalse(retryConsumerDidFail.WaitOne(maxWaitTime), "The retry consumer did unexpectedly fail with an exception.");
        }

        [TestMethod]
        public void TestMultiConsumer_DistributeEvents()
        {
            /* PRECONDITION */
            Debug.Assert(defaultMaxConsumer > 1);
            Debug.Assert(defaultMaxEvents > 0);
            Debug.Assert(Strategy != null);
            Debug.Assert(Strategy.IsClosed);

            var consumersReceivedEventsIndividually = new CountdownEvent(defaultMaxConsumer);
            var producerFinished = new ManualResetEvent(false);
            var maxEvents = defaultMaxEvents / 2;

            /* GIVEN */
            Strategy.Open();
            for (var index = 0; index < defaultMaxConsumer; index++)
            {
                var consumer = new TestConsumer(Strategy);
                consumer.Consume(
                    false,
                    (@event, index) => index < maxEvents,
                    result =>
                    {
                        if (result != null && result.IsSuccess()) consumersReceivedEventsIndividually.Signal();
                    });
            }

            /* WHEN */
            var producer = new TestProducer(Strategy);
            producer.Produce(false, num => num < maxEvents, result => result.EventSuccess(producerFinished));

            /* THEN */
            Assert.IsTrue(producerFinished.WaitOne(maxWaitTime), "Producer was not able to queue all events!");
            Assert.IsTrue(consumersReceivedEventsIndividually.Wait(maxWaitTime * defaultMaxConsumer), "Not all consumers received the event!");
        }
    }
}
