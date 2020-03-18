using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Data.IntermediateFormat.Json;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Events.Queue.Strategy.SingleConsumer;
using SharedTest.TestHelpers.Event;
using System.Text.Json;
using SharedTest.TestHelpers.Utility;

namespace MORRTest.Data.IntermediateFormat.Json
{
    /// <summary>
    /// Tests the functionality of the JsonIntermediateFormatDeserializer
    /// by using test producers and consumers to inject test events.
    /// </summary>
    [TestClass]
    public class JsonIntermediateFormatDeserializerTest
    {
        public class SupportDeserializationEventQueueImp : SupportDeserializationEventQueue<TestEvent>
        {
            public SupportDeserializationEventQueueImp() : base(new UnboundedSingleConsumerChannelStrategy<TestEvent>()) { }
        }

        public class DecodableEventQueueImp : DefaultDecodableEventQueue<JsonIntermediateFormatSample> { }

        private JsonIntermediateFormatDeserializer deserializer;
        private SupportDeserializationEventQueue<TestEvent> outputQueue;
        private DefaultDecodableEventQueue<JsonIntermediateFormatSample> inputQueue;
        private CompositionContainer container;
        private const int maxWaitTime = 1000;

        [TestInitialize]
        public void BeforeTest()
        {
            deserializer = new JsonIntermediateFormatDeserializer();

            inputQueue = new DecodableEventQueueImp();
            inputQueue.Open();

            outputQueue = new SupportDeserializationEventQueueImp();
            outputQueue.Open();

            container = new CompositionContainer();
        }

        [TestCleanup]
        public void AfterTest()
        {
            inputQueue.Close();
            outputQueue.Close();

            inputQueue = null;
            outputQueue = null;
            deserializer = null;

            container.Dispose();
        }

        /// <summary>
        /// Tests if activating the deserializer correctly enables it functionality.
        /// </summary>
        [TestMethod]
        public void TestJsonIntermediateFormatDeserializer_SetActive()
        {
            /* PRECONDITION */
            Debug.Assert(inputQueue != null);
            Debug.Assert(outputQueue != null);
            Debug.Assert(deserializer != null);
            Debug.Assert(container != null);
            Debug.Assert(!deserializer.IsActive);

            /* GIVEN */
            // Load all MEF instances correctly
            container.ComposeExportedValue<ISupportDeserializationEventQueue<Event>>(outputQueue);
            container.ComposeExportedValue<IDecodableEventQueue<JsonIntermediateFormatSample>>(inputQueue);
            container.ComposeParts(deserializer);

            const int identifier = 404;
            var @event = new TestEvent(identifier);
            using var outputReceivedEvent = new ManualResetEvent(false);
            var sample = new JsonIntermediateFormatSample
            {
                Data = JsonSerializer.SerializeToUtf8Bytes(@event),
                Timestamp = @event.Timestamp,
                Type = @event.GetType()
            };

            /* WHEN */
            deserializer.IsActive = true;
            Assert.IsTrue(deserializer.IsActive, "Deserializer did not activate correctly!");

            ExpectOutput(outputQueue, (received) =>
            {
                if (received == null || !received.Equals(@event)) return false;
                outputReceivedEvent.Set();
                return true;
            });

            inputQueue.Enqueue(sample);

            /* THEN */ 
            Assert.IsTrue(outputReceivedEvent.WaitOne(maxWaitTime), "Did not receive serialized event in time!");
        }

        /// <summary>
        /// Tests whether the deserializer closes correctly if the input queue closes.
        /// </summary>
        [TestMethod]
        public void TestJsonIntermediateFormatDeserializer_DeactivatesOnClose()
        {
            /* PRECONDITION */
            Debug.Assert(inputQueue != null);
            Debug.Assert(outputQueue != null);
            Debug.Assert(deserializer != null);
            Debug.Assert(container != null);
            Debug.Assert(!deserializer.IsActive);

            /* GIVEN */
            // Load all MEF instances correctly
            container.ComposeExportedValue<ISupportDeserializationEventQueue<Event>>(outputQueue);
            container.ComposeExportedValue<IDecodableEventQueue<JsonIntermediateFormatSample>>(inputQueue);
            container.ComposeParts(deserializer);

            var @event = new TestEvent();
            var sample = new JsonIntermediateFormatSample
            {
                Data = JsonSerializer.SerializeToUtf8Bytes(@event),
                Timestamp = @event.Timestamp,
                Type = @event.GetType()
            };

            using var outputReceivedEvent = new ManualResetEvent(false);
            using var didCloseEvent = new ManualResetEvent(false);

            /* WHEN */
            deserializer.IsActive = true;
            Assert.IsTrue(deserializer.IsActive, "Deserializer did not activate correctly!");

            ExpectOutput(outputQueue, (_) => outputReceivedEvent.Set(), () => didCloseEvent.Set());

            inputQueue.Enqueue(sample);
            Assert.IsTrue(outputReceivedEvent.WaitOne(maxWaitTime), "Did not receive serialized event in time!");

            inputQueue.Close();

            /* THEN */
            Assert.IsTrue(inputQueue.IsClosed, "InputQueue failed to close!");
            Assert.IsTrue(didCloseEvent.WaitOne(maxWaitTime), "Did not close output queue in time!");
        }

        private static void ExpectOutput<T>(EventQueue<T> queue, Func<T, bool> predicate, Action completeAction = null) where T : Event
        {
            using var awaitThreadStartEvent = new ManualResetEvent(false);
            var thread = new Thread(async () =>
            {
                await foreach (var @event in Awaitable.Await(queue.GetEvents(), awaitThreadStartEvent))
                {
                    if (predicate.Invoke(@event)) break;
                }

                completeAction?.Invoke();
            });

            thread.Start();
            Assert.IsTrue(awaitThreadStartEvent.WaitOne(maxWaitTime), "Thread did not start in time!");
        }
    }
}
