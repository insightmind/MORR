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
        private MouseModule mouseModule;
        private Mock<MouseClickEventProducer> mouseClickEventProducer;
        private Mock<MouseMoveEventProducer> mouseMoveEventProducer;
        private Mock<MouseScrollEventProducer> mouseScrollEventProducer;
        private MouseModuleConfiguration mouseModuleConfiguration;
        private CompositionContainer container;

        [TestInitialize]
        public void BeforeTest()
        {
            mouseModule = new MouseModule();
            mouseClickEventProducer = new Mock<MouseClickEventProducer>();
            mouseMoveEventProducer = new Mock<MouseMoveEventProducer>();
            mouseScrollEventProducer = new Mock<MouseScrollEventProducer>();
            mouseModuleConfiguration = new MouseModuleConfiguration() {SamplingRateInHz = 10, Threshold = 50};
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
    }
}