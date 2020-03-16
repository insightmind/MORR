using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Data.IntermediateFormat.Json;
using MORR.Core.Data.Transcoding.Json;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Events.Queue.Strategy;
using MORR.Shared.Events.Queue.Strategy.SingleConsumer;
using Newtonsoft.Json;
using SharedTest.Events;
using SharedTest.Events.Queue;
using SharedTest.TestHelpers.Event;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
            var outputReceivedEvent = new ManualResetEvent(false);
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

            var didCompleteEvent = new ManualResetEvent(false);

            deserializer.IsActive = true;

            /* WHEN */
            ExpectOutput(outputQueue, (_) => true, () => didCompleteEvent.Set());

            inputQueue.Close();
            Assert.IsTrue(inputQueue.IsClosed);

            /* THEN */
            Assert.IsTrue(didCompleteEvent.WaitOne(maxWaitTime), "Did not complete in time!");
            Assert.IsTrue(outputQueue.IsClosed);
        }

        private static void ExpectOutput<T>(EventQueue<T> queue, Func<T, bool> predicate, Action completeAction = null) where T : Event
        {
            var task = new Task(async () =>
            {
                await foreach (var @event in queue.GetEvents())
                {
                    if (predicate.Invoke(@event)) break;
                }

                completeAction?.Invoke();
            });

            task.RunSynchronously();
        }
    }
}
