using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Data.Transcoding.Json;
using MORR.Shared.Utility;
using MORRTest.TestHelper.Json;
using SharedTest.TestHelpers.Event;
using SharedTest.TestHelpers.Utility;

namespace MORRTest.Data.Transcoding.Json
{
    [TestClass]
    public class JsonDecoderTest
    {
        private JsonDecoder decoder;
        private MockFileSystem fileSystem;
        private JsonDecoderConfiguration config;
        private CompositionContainer container;
        private const string recordingFolderLocation = "C:\\";
        private const string fileLocation = "somejson.json";

        private const int maxWaitTime = 1000;

        [TestInitialize]
        public void BeforeTest()
        {
            fileSystem = new MockFileSystem();
            decoder = new JsonDecoder(fileSystem);
            config = new JsonDecoderConfiguration();
            container = new CompositionContainer();
        }

        [TestCleanup]
        public void AfterTest()
        {
            container.Dispose();
            container = null;
            config = null;
            fileSystem = null;
            decoder = null;
        }

        /// <summary>
        /// Tests whether the JsonDecoder can decode a correct file.
        /// </summary>
        [DoNotParallelize]
        [TestMethod]
        public void TestJsonDecoder_ParseSuccess()
        {
            /* PRECONDITION */
            Debug.Assert(decoder != null);
            Debug.Assert(fileSystem != null);
            Debug.Assert(config != null);
            Debug.Assert(container != null);

            config.RelativeFilePath = new FilePath(fileLocation, true);

            container.ComposeExportedValue(config);
            container.ComposeParts(decoder);

            /* GIVEN */
            var random = new Random();
            var eventList = new List<TestEvent>
            {
                new TestEvent(random.Next()),
                new TestEvent(random.Next())
            };

            var serializedString = JsonManualSerialization.GenerateSerializedEvents(eventList);
            var mockFileData = new MockFileData(serializedString, Encoding.UTF8);
            fileSystem.AddFile(recordingFolderLocation + fileLocation, mockFileData);

            using var awaitThreadEvent = new ManualResetEvent(false);
            using var openEvent = new ManualResetEvent(false);
            using var didDecodeAllEvents = new ManualResetEvent(false);

            /* WHEN */
            var thread = new Thread(async () =>
            {
                await foreach (var @event in Awaitable.Await(decoder.GetEvents(), awaitThreadEvent))
                {
                    var deserialized = JsonSerializer.Deserialize(@event.Data, @event.Type);

                    if ((!(deserialized is TestEvent receivedEvent))) continue;

                    var index = eventList.FindIndex((compareEvent) => compareEvent.Identifier == receivedEvent.Identifier);
                    if (index == -1) continue;

                    eventList.RemoveAt(index);

                    if (eventList.Count == 0)
                    {
                        didDecodeAllEvents.Set();
                    }
                }
            });

            decoder.Open();
            thread.Start();
            Assert.IsTrue(awaitThreadEvent.WaitOne(maxWaitTime), "Thread did not start in time!");

            decoder.Decode(new DirectoryPath(recordingFolderLocation, true));

            /* THEN */
            Assert.IsTrue(decoder.DecodeFinished.WaitOne(maxWaitTime), "Decoder did not finish in time!");
            Assert.IsTrue(didDecodeAllEvents.WaitOne(maxWaitTime), "Could not find all encoded events.");
        }

        [TestMethod]
        public void TestJsonDecoder_ClosesOnCompletion()
        {
            /* PRECONDITION */
            Debug.Assert(decoder != null);
            Debug.Assert(fileSystem != null);
            Debug.Assert(config != null);
            Debug.Assert(container != null);

            config.RelativeFilePath = new FilePath(fileLocation, true);

            container.ComposeExportedValue(config);
            container.ComposeParts(decoder);

            /* GIVEN */
            decoder.Open();

            /* WHEN */
            decoder.DecodeFinished.Set();
            var decodeEvent = decoder.DecodeFinished;

            /* THEN */
            Assert.IsTrue(decodeEvent.WaitOne(maxWaitTime), "Decode Event still not complete!");
            Assert.IsTrue(decoder.IsClosed, "Decoder has not closed in time!");
        }
    }
}
