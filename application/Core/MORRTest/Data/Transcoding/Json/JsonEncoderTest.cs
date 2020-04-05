using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Data.IntermediateFormat.Json;
using MORR.Core.Data.Transcoding.Json;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;
using MORRTest.TestHelper.Json;
using SharedTest.TestHelpers.Event;
using SharedTest.TestHelpers.Utility;

namespace MORRTest.Data.Transcoding.Json
{
    [TestClass]
    public class JsonEncoderTest
    {
        /// <summary>
        /// This helper class allows us to create an instance of the DefaultEncodableEventQueue.
        /// </summary>
        public class DefaultEncodeEventQueueImp : DefaultEncodableEventQueue<JsonIntermediateFormatSample> { }

        /// <summary>
        /// However as we cannot override the GetEvents method on an existing implementation because the method is not
        /// marked as virtual.
        /// Therefore we have another test class which we use to make sure the encoder attaches correctly.
        /// </summary>
        public class EncodeEventQueueImp : IEncodableEventQueue<JsonIntermediateFormatSample>
        {
            private readonly DefaultEncodeEventQueueImp innerQueue = new DefaultEncodeEventQueueImp();

            public readonly ManualResetEvent ConsumerAttachedEvent;

            public bool IsClosed => innerQueue.IsClosed;

            public void Open() => innerQueue.Open();

            public void Close() => innerQueue.Close();

            public void Enqueue(JsonIntermediateFormatSample @event) => innerQueue.Enqueue(@event);

            public IAsyncEnumerable<JsonIntermediateFormatSample> GetEvents() => Awaitable.Await(innerQueue.GetEvents(), ConsumerAttachedEvent);

            public EncodeEventQueueImp()
            {
                ConsumerAttachedEvent = new ManualResetEvent(false);
            }

            ~EncodeEventQueueImp()
            {
                ConsumerAttachedEvent.Dispose();
            }
        }

        private JsonEncoder encoder;
        private IFileSystem fileSystem;
        private JsonEncoderConfiguration config;
        private CompositionContainer container;
        private EncodeEventQueueImp input;
        private const string recordingFolderLocation = @"C:\";
        private const string fileLocation = "somejson.json";

        private const int maxWaitTime = 100000;

        [TestInitialize]
        public void BeforeTest()
        {
            container = new CompositionContainer();
            fileSystem = new MockFileSystem();
            encoder = new JsonEncoder(fileSystem);
            config = new JsonEncoderConfiguration();

            input = new EncodeEventQueueImp();
            input.Open();
        }

        [TestCleanup]
        public void AfterTest()
        {
            container.Dispose();
            container = null;

            fileSystem = null;
            encoder = null;
            config = null;

            input = null;
        }
        
        /// <summary>
        /// Tests whether the encoder successfully encodes the input events.
        /// </summary>
        [TestMethod]
        public void TestJsonEncoder_EncodeSuccess()
        {
            /* PRECONDITION */
            Debug.Assert(encoder != null);
            Debug.Assert(fileSystem != null);
            Debug.Assert(config != null);
            Debug.Assert(container != null);
            Debug.Assert(input != null);

            container.ComposeExportedValue<IEncodableEventQueue<JsonIntermediateFormatSample>>(input);
            container.ComposeExportedValue(config);
            container.ComposeParts(encoder);

            /* GIVEN */
            var random = new Random();
            var eventList = new List<TestEvent>
            {
                new TestEvent(random.Next()),
                new TestEvent(random.Next())
            };

            var serializedEvent = JsonManualSerialization.GenerateSerializedEvents(eventList);

            config.RelativeFilePath = new FilePath(fileLocation, true);

            /* WHEN */
            encoder.Encode(new DirectoryPath(recordingFolderLocation, true));

            // As the input queue is correctly attached we can now queue our events.
            Assert.IsTrue(input.ConsumerAttachedEvent.WaitOne(maxWaitTime), "Encoder did not attach to input queue in time!");

            // We enqueue now all events we want to serialize.
            foreach (var @event in eventList)
            {
                var sample = new JsonIntermediateFormatSample()
                {
                    Data = JsonSerializer.SerializeToUtf8Bytes(@event),
                    IssuingModule = @event.IssuingModule,
                    Timestamp = @event.Timestamp,
                    Type = @event.GetType()
                };

                input.Enqueue(sample);
            }

            input.Close();

            Assert.IsTrue(encoder.EncodeFinished.WaitOne(maxWaitTime), "Encoder did not finish in time!");

            /* THEN */
            // We get the encoded string which the encoder produced.
            const string path = recordingFolderLocation + fileLocation;
            Assert.IsTrue(fileSystem.File.Exists(path));

            var encodedString = fileSystem.File.ReadAllText(recordingFolderLocation + fileLocation, Encoding.Default);
            var plusResolvedString = encodedString.Replace(@"\u002B", "+");
            Assert.AreEqual(serializedEvent, plusResolvedString);
        }
    }
}
