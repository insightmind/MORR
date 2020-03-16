using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Data.IntermediateFormat.Json;
using MORR.Shared.Events.Queue;
using SharedTest.TestHelpers.Event;

namespace MORRTest.Data.IntermediateFormat.Json
{
    [TestClass]
    public class JsonIntermediateFormatSerializerTest
    {
        public class NonDeserializableEventQueueImp : NonDeserializableEventQueue<TestEvent> { }

        private JsonIntermediateFormatSerializer serializer;
        private NonDeserializableEventQueueImp inputQueue;
        private CompositionContainer container;

        [TestInitialize]
        public void BeforeTest()
        {
            serializer = new JsonIntermediateFormatSerializer();
            inputQueue = new NonDeserializableEventQueueImp();
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

        [TestMethod]
        public void TestJsonIntermediateFormatSerializer_SetActive()
        {
            /* PRECONDITION */

            /* GIVEN */

            /* WHEN */

            /* THEN */
        }

        [TestMethod]
        public void TestJsonIntermediateFormatSerializer_SetInactive()
        {
            /* PRECONDITION */

            /* GIVEN */

            /* WHEN */

            /* THEN */
        }
    }
}
