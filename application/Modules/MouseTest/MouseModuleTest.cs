using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.Mouse;
using MORR.Modules.Mouse.Producers;

namespace MouseTest
{
    [TestClass]
    public class MouseModuleTest
    {
        private CompositionContainer container;
        private Mock<MouseClickEventProducer> mouseClickEventProducer;
        private MouseModule mouseModule;
        private MouseModuleConfiguration mouseModuleConfiguration;
        private Mock<MouseMoveEventProducer> mouseMoveEventProducer;
        private Mock<MouseScrollEventProducer> mouseScrollEventProducer;

        [TestInitialize]
        public void BeforeTest()
        {
            mouseModule = new MouseModule();
            mouseClickEventProducer = new Mock<MouseClickEventProducer>();
            mouseMoveEventProducer = new Mock<MouseMoveEventProducer>();
            mouseScrollEventProducer = new Mock<MouseScrollEventProducer>();
            mouseModuleConfiguration = new TestMouseModuleConfiguration();
            container = new CompositionContainer();
            container.ComposeExportedValue(mouseClickEventProducer.Object);
            container.ComposeExportedValue(mouseMoveEventProducer.Object);
            container.ComposeExportedValue(mouseScrollEventProducer.Object);
            container.ComposeExportedValue(mouseModuleConfiguration);
            container.ComposeParts(mouseModule);
        }

        [TestMethod]
        public void TestMouseModule_Activate()
        {
            // Preconditions
            Debug.Assert(mouseModule != null);

            /* GIVEN */

            /* WHEN */
            mouseModule.Initialize(true);
            mouseModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(mouseModule.IsActive);
        }

        [TestMethod]
        public void TestMouseModule_Deactivate()
        {
            // Preconditions
            Debug.Assert(mouseModule != null);

            /* GIVEN */

            /* WHEN */
            mouseModule.IsActive = false;

            /* THEN */
            Assert.IsFalse(mouseModule.IsActive);
        }

        private class TestMouseModuleConfiguration : MouseModuleConfiguration
        {
            public TestMouseModuleConfiguration()
            {
                SamplingRateInHz = 10;
                Threshold = 50;
            }
        }
    }
}