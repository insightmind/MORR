using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Modules.WindowManagement;
using MORR.Modules.WindowManagement.Producers;
using MORR.Shared.Hook;
using SharedTest.TestHelpers.INativeHook;

namespace WindowManagementTest
{
    [TestClass]
    public class WindowManagementModuleTest
    {
        private readonly GlobalHook.MessageType[] windowEventListenedMessagesTypes =
        {
            GlobalHook.MessageType.WM_ACTIVATE,
            GlobalHook.MessageType.WM_ENTERSIZEMOVE,
            GlobalHook.MessageType.WM_EXITSIZEMOVE,
            GlobalHook.MessageType.WM_SIZE
        };

        private CompositionContainer container;
        private HookNativeMethodsMock hookNativeMethods;
        private WindowFocusEventProducer windowFocusEventProducer;
        private WindowManagementModule windowManagementModule;
        private WindowMovementEventProducer windowMovementEventProducer;
        private WindowResizingEventProducer windowResizingEventProducer;
        private WindowStateChangedEventProducer windowStateChangedEventProducer;


        [TestInitialize]
        public void BeforeTest()
        {
            windowManagementModule = new WindowManagementModule();
            windowFocusEventProducer = new WindowFocusEventProducer();
            windowMovementEventProducer = new WindowMovementEventProducer();
            windowResizingEventProducer = new WindowResizingEventProducer();
            windowStateChangedEventProducer = new WindowStateChangedEventProducer();

            container = new CompositionContainer();
            container.ComposeExportedValue(windowFocusEventProducer);
            container.ComposeExportedValue(windowMovementEventProducer);
            container.ComposeExportedValue(windowResizingEventProducer);
            container.ComposeExportedValue(windowStateChangedEventProducer);
            container.ComposeParts(windowManagementModule);

            hookNativeMethods = new HookNativeMethodsMock();
            hookNativeMethods.Initialize();
        }

        [TestCleanup]
        public void AfterTest()
        {
            windowManagementModule = null;
            windowMovementEventProducer = null;
            windowFocusEventProducer = null;
            windowStateChangedEventProducer = null;
            windowResizingEventProducer = null;
            container.Dispose();
            container = null;
            hookNativeMethods = null;
        }

        [TestMethod]
        public void TestWindowManagementModule_Activate()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowFocusEventProducer != null);
            Debug.Assert(windowMovementEventProducer != null);
            Debug.Assert(windowResizingEventProducer != null);
            Debug.Assert(windowStateChangedEventProducer != null);
            Debug.Assert(hookNativeMethods != null);

            /* GIVEN */

            /* WHEN */
            windowManagementModule.Initialize(true);
            foreach (var messageType in windowEventListenedMessagesTypes)
            {
                hookNativeMethods.AllowMessageTypeRegistry(messageType);
            }

            hookNativeMethods.AllowLibraryLoad();
            windowManagementModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(windowManagementModule.IsActive);
            windowManagementModule.IsActive = false;
        }

        [TestMethod]
        public void TestWindowManagementModule_Deactivate()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowFocusEventProducer != null);
            Debug.Assert(windowMovementEventProducer != null);
            Debug.Assert(windowResizingEventProducer != null);
            Debug.Assert(windowStateChangedEventProducer != null);

            /* GIVEN */

            /* WHEN */
            windowManagementModule.Initialize(true);
            windowManagementModule.IsActive = false;

            /* THEN */
            Assert.IsFalse(windowManagementModule.IsActive);
        }

        [TestMethod]
        public void TestWindowManagementModule_InitializedFalse_ChannelClosed()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowFocusEventProducer != null);
            Debug.Assert(windowMovementEventProducer != null);
            Debug.Assert(windowResizingEventProducer != null);
            Debug.Assert(windowStateChangedEventProducer != null);

            /* GIVEN */

            /* WHEN */
            windowManagementModule.Initialize(false);

            /* THEN */
            Assert.IsTrue(windowFocusEventProducer.IsClosed);
            Assert.IsTrue(windowMovementEventProducer.IsClosed);
            Assert.IsTrue(windowResizingEventProducer.IsClosed);
            Assert.IsTrue(windowStateChangedEventProducer.IsClosed);
        }

        [TestMethod]
        public void TestWindowManagementModule_InitializedTrue_ChannelOpened()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowFocusEventProducer != null);
            Debug.Assert(windowMovementEventProducer != null);
            Debug.Assert(windowResizingEventProducer != null);
            Debug.Assert(windowStateChangedEventProducer != null);

            /* GIVEN */

            /* WHEN */
            windowManagementModule.Initialize(true);

            /* THEN */
            Assert.IsFalse(windowFocusEventProducer.IsClosed);
            Assert.IsFalse(windowMovementEventProducer.IsClosed);
            Assert.IsFalse(windowResizingEventProducer.IsClosed);
            Assert.IsFalse(windowStateChangedEventProducer.IsClosed);
        }
    }
}