﻿using System;
using System.Collections.Generic;
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
using SharedTest.TestHelpers.Utility;

namespace MORRTest.Data.IntermediateFormat.Json
{
    [TestClass]
    public class JsonIntermediateFormatSerializerTest
    {
        public class DefaultNonDeserializableEventQueueImp : NonDeserializableEventQueue<TestEvent> { }

        /// <summary>
        /// However as we cannot override the GetEvents method on an existing implementation because the method is not
        /// marked as virtual.
        /// Therefore we have another test class which we use to make sure the encoder attaches correctly.
        /// </summary>
        public class NonDeserializableEventQueueImp : IReadOnlyEventQueue<TestEvent>
        {
            private readonly DefaultNonDeserializableEventQueueImp innerQueue = new DefaultNonDeserializableEventQueueImp();

            public readonly ManualResetEvent ConsumerAttachedEvent;

            public bool IsClosed => innerQueue.IsClosed;

            public void Open() => innerQueue.Open();

            public void Close() => innerQueue.Close();

            public void Enqueue(TestEvent @event) => innerQueue.Enqueue(@event);

            public IAsyncEnumerable<TestEvent> GetEvents() => Awaitable.Await(innerQueue.GetEvents(), ConsumerAttachedEvent);

            public NonDeserializableEventQueueImp()
            {
                ConsumerAttachedEvent = new ManualResetEvent(false);
            }

            ~NonDeserializableEventQueueImp()
            {
                ConsumerAttachedEvent.Dispose();
            }
        }

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
            using var didConsumeEvent = new ManualResetEvent(false);
            using var didStartConsumingEvent = new ManualResetEvent(false);

            /* WHEN */
            serializer.IsActive = true;
            Assert.IsTrue(serializer.IsActive);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Awaitable.Await(serializer.GetEvents(), didStartConsumingEvent))
                {
                    var deserialized = JsonSerializer.Deserialize(@event.Data, @event.Type);

                    if ((deserialized is TestEvent receivedEvent) && receivedEvent.Identifier == @testEvent.Identifier)
                    {
                        didConsumeEvent.Set();
                    }
                }
            });

            thread.Start(); 
            Assert.IsTrue(didStartConsumingEvent.WaitOne(maxWaitTime));
            Assert.IsTrue(inputQueue.ConsumerAttachedEvent.WaitOne(maxWaitTime), "Serializer did not attach in time to input queue!");
            inputQueue.Enqueue(@testEvent);

            /* THEN */ 
            Assert.IsFalse(serializer.IsClosed, "Serializer unexpectedly still closed!"); 
            Assert.IsTrue(didConsumeEvent.WaitOne(maxWaitTime), "Did not receive serialized event in time!");
        }
    }
}
