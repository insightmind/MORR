using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.WindowManagement;
using MORR.Modules.WindowManagement.Producers;

namespace WindowManagementTest
{
    [TestClass]
    public class WindowManagementModuleTest
    {
        private WindowManagementModule windowManagementModule;
        private Mock<WindowFocusEventProducer> windowFocusEventProducer;
        private Mock<WindowMovementEventProducer> windowMovementEventProducer;
        private Mock<WindowResizingEventProducer> windowResizingEventProducer;
        private Mock<WindowStateChangedEventProducer> windowStateChangedEventProducer;
        private CompositionContainer container;

        [TestInitialize]
        public void BeforeTest()
        {
            windowManagementModule = new WindowManagementModule();
            windowFocusEventProducer = new Mock<WindowFocusEventProducer>();
            windowMovementEventProducer = new Mock<WindowMovementEventProducer>();
            windowResizingEventProducer = new Mock<WindowResizingEventProducer>();
            windowStateChangedEventProducer = new Mock<WindowStateChangedEventProducer>();
            container = new CompositionContainer();
            container.ComposeExportedValue(windowFocusEventProducer.Object);
            container.ComposeExportedValue(windowMovementEventProducer.Object);
            container.ComposeExportedValue(windowResizingEventProducer.Object);
            container.ComposeExportedValue(windowStateChangedEventProducer.Object);
            container.ComposeParts(windowResizingEventProducer);
        }

        [TestMethod]
        public void TestMouseModule_Activate()
        {
            // Preconditions
            Debug.Assert(windowManagementModule != null);

            /* GIVEN */

            /* WHEN */
            windowManagementModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(windowManagementModule.IsActive);
        }

        [TestMethod]
        public void TestMouseModule_Deactivate()
        {
            // Preconditions
            Debug.Assert(windowManagementModule != null);

            /* GIVEN */

            /* WHEN */
            windowManagementModule.IsActive = false;

            /* THEN */
            Assert.IsFalse(windowManagementModule.IsActive);
        }
    }
}