using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.WindowManagement;
using MORR.Modules.WindowManagement.Producers;
using MORR.Shared.Hook;

namespace WindowManagementTest
{
    [TestClass]
    public class WindowManagementModuleTest
    {
        private CompositionContainer container;
        private Mock<WindowFocusEventProducer> windowFocusEventProducer;
        private WindowManagementModule windowManagementModule;
        private Mock<WindowMovementEventProducer> windowMovementEventProducer;
        private Mock<WindowResizingEventProducer> windowResizingEventProducer;
        private Mock<WindowStateChangedEventProducer> windowStateChangedEventProducer;

        private Mock<IHookNativeMethods> nativeHook;

        [TestInitialize]
        public void BeforeTest()
        {
            windowManagementModule = new WindowManagementModule();
            windowFocusEventProducer = new Mock<WindowFocusEventProducer>();
            windowMovementEventProducer = new Mock<WindowMovementEventProducer>();
            windowResizingEventProducer = new Mock<WindowResizingEventProducer>();
            windowStateChangedEventProducer = new Mock<WindowStateChangedEventProducer>();
            nativeHook = new Mock<IHookNativeMethods>();

            container = new CompositionContainer();
            container.ComposeExportedValue(windowFocusEventProducer.Object);
            container.ComposeExportedValue(windowMovementEventProducer.Object);
            container.ComposeExportedValue(windowResizingEventProducer.Object);
            container.ComposeExportedValue(windowStateChangedEventProducer.Object);
            container.ComposeParts(windowManagementModule);

            nativeHook
                .Setup(hook => hook.Capture(It.IsAny<uint>()))?
                .Returns(true);

            nativeHook
                .Setup(mock => mock.LoadLibrary())?
                .Returns(new IntPtr(0x1)); // We just return a non null pointer

            GlobalHook.Initialize(nativeHook.Object);
        }

        [TestMethod]
        public void TestWindowManagementModule_Activate()
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
        public void TestWindowManagementModule_Deactivate()
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