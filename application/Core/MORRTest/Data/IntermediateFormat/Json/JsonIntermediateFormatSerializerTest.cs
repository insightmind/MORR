using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Data.IntermediateFormat.Json;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using SharedTest.TestHelpers.Event;
using System.Text.Json;

namespace MORRTest.Data.IntermediateFormat.Json
{
    [TestClass]
    public class JsonIntermediateFormatSerializerTest
    {
        public class NonDeserializableEventQueueImp : NonDeserializableEventQueue<TestEvent> { }

        private JsonIntermediateFormatSerializer serializer;
        private NonDeserializableEventQueueImp inputQueue;
        private CompositionContainer container;

        private const int maxWaitTime = 500;

        [TestInitialize]
        public void BeforeTest()
        {
            serializer = new JsonIntermediateFormatSerializer();
            inputQueue = new NonDeserializableEventQueueImp();
            inputQueue.Open();

            container = new CompositionContainer();
        }

        [TestCleanup]
        public void AfterTest()
        {
            container.Dispose();
            container = null;
            serializer = null;
            inputQueue = null;
        }

        /// <summary>
        /// Tests if the initialization correctly opens the transforming module.
        /// </summary>
        [TestMethod]
        public void TestJsonIntermediateFormatSerializer_InitializeOpens()
        {
            /* PRECONDITION */
            Debug.Assert(serializer != null);
            Debug.Assert(inputQueue != null);
            Debug.Assert(container != null);
            Debug.Assert(serializer.IsClosed);

            /* GIVEN */
            container.ComposeExportedValue<IReadOnlyEventQueue<Event>>(inputQueue);
            container.ComposeParts(serializer);

            /* WHEN */
            serializer.Initialize(true);

            /* THEN */
            Assert.IsFalse(serializer.IsClosed, "Serializer unexpectedly still closed!");
        }

        /// <summary>
        /// Tests if the initialization correctly opens the transforming module.
        /// </summary>
        [TestMethod]
        public void TestJsonIntermediateFormatSerializer_InitializeCloses()
        {
            /* PRECONDITION */
            Debug.Assert(serializer != null);
            Debug.Assert(inputQueue != null);
            Debug.Assert(container != null);
            Debug.Assert(serializer.IsClosed);

            /* GIVEN */
            container.ComposeExportedValue<IReadOnlyEventQueue<Event>>(inputQueue);
            container.ComposeParts(serializer);

            /* WHEN */
            serializer.Initialize(true);
            Assert.IsFalse(serializer.IsClosed, "Serializer unexpectedly still closed!");

            serializer.Initialize(false);
            Assert.IsTrue(serializer.IsClosed, "Serializer unexpectedly still closed!");
        }

        /// <summary>
        /// Tests if the initialization correctly opens the transforming module.
        /// </summary>
        [TestMethod]
        public void TestJsonIntermediateFormatSerializer_ClosesOnInputClose()
        {
            /* PRECONDITION */
            Debug.Assert(serializer != null);
            Debug.Assert(inputQueue != null);
            Debug.Assert(container != null);
            Debug.Assert(serializer.IsClosed);
            Debug.Assert(!inputQueue.IsClosed);

            /* GIVEN */
            container.ComposeExportedValue<IReadOnlyEventQueue<Event>>(inputQueue);
            container.ComposeParts(serializer);

            /* WHEN */
            serializer.Initialize(true);
            serializer.IsActive = true;
            Assert.IsFalse(serializer.IsClosed, "Serializer unexpectedly still closed!");

            inputQueue.Close();
            Assert.IsTrue(inputQueue.IsClosed);

            SpinWait.SpinUntil(() => serializer.IsClosed);
            Assert.IsTrue(serializer.IsClosed, "Serializer did not close in time!");
        }

        /// <summary>
        /// Test if queueing an event into an input queue results into a
        /// correct serialized output event in the serializer.
        /// </summary>
        [TestMethod]
        public void TestJsonIntermediateFormatSerializer_SetActive()
        {
            /* PRECONDITION */
            Debug.Assert(serializer != null);
            Debug.Assert(inputQueue != null);
            Debug.Assert(container != null);
            Debug.Assert(!serializer.IsActive);
            Debug.Assert(serializer.IsClosed);

            /* GIVEN */
            container.ComposeExportedValue<IReadOnlyEventQueue<Event>>(inputQueue);
            container.ComposeParts(serializer);

            serializer.Initialize(true);

            const int identifier = 404;
            var @testEvent = new TestEvent(identifier);
            var didConsumeEvent = new ManualResetEvent(false);

            /* WHEN */
            serializer.IsActive = true;
            Assert.IsTrue(serializer.IsActive);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in serializer.GetEvents())
                {
                    var deserialized = JsonSerializer.Deserialize(@event.Data, @event.Type);

                    if ((deserialized is TestEvent receivedEvent) && receivedEvent.Identifier == @testEvent.Identifier)
                    {
                        didConsumeEvent.Set();
                    }
                }
            });

            thread.Start();
            inputQueue.Enqueue(@testEvent);

            /* THEN */
            Assert.IsFalse(serializer.IsClosed, "Serializer unexpectedly still closed!");
            Assert.IsTrue(didConsumeEvent.WaitOne(maxWaitTime), "Did not receive serialized event in time!");
        }
    }
}
