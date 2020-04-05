using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.Mouse;
using MORR.Modules.Mouse.Producers;
using MORR.Shared.Hook;
using SharedTest.TestHelpers.INativeHook;
using MORR.Modules.Mouse.Native;

namespace MouseTest
{
    [TestClass]
    public class MouseModuleTest
    {
        protected const int maxWaitTime = 5000;

        private class TestMouseModuleConfiguration : MouseModuleConfiguration
        {
            public TestMouseModuleConfiguration()
            {
                SamplingRateInHz = 10;
                Threshold = 50;
            }
        }
        private CompositionContainer container;
        private MouseClickEventProducer mouseClickEventProducer;
        private MouseMoveEventProducer mouseMoveEventProducer;
        private MouseScrollEventProducer mouseScrollEventProducer;
        private MouseModule mouseModule;
        private MouseModuleConfiguration mouseModuleConfiguration;
        private Mock<INativeMouse> nativeMouseMock;
        private HookNativeMethodsMock hookNativeMethodsMock;

        private readonly GlobalHook.MessageType[] mouseClickListenedMessagesTypes =
        {
            GlobalHook.MessageType.WM_RBUTTONDOWN,
            GlobalHook.MessageType.WM_LBUTTONDOWN,
            GlobalHook.MessageType.WM_MBUTTONDOWN,
            GlobalHook.MessageType.WM_RBUTTONDBLCLK,
            GlobalHook.MessageType.WM_LBUTTONDBLCLK,
            GlobalHook.MessageType.WM_MBUTTONDBLCLK,
            GlobalHook.MessageType.WM_NCRBUTTONDOWN,
            GlobalHook.MessageType.WM_NCLBUTTONDOWN,
            GlobalHook.MessageType.WM_NCMBUTTONDOWN,
            GlobalHook.MessageType.WM_NCRBUTTONDBLCLK,
            GlobalHook.MessageType.WM_NCLBUTTONDBLCLK,
            GlobalHook.MessageType.WM_NCMBUTTONDBLCLK
        };

        private readonly GlobalHook.MessageType[] mouseScrollListenedMessagesTypes =
        {
            GlobalHook.MessageType.WM_MOUSEWHEEL
        };

        [TestInitialize]
        public void BeforeTest()
        {
            //initialize module, producers and configuration
            mouseModule = new MouseModule();
            mouseClickEventProducer = new MouseClickEventProducer();
            mouseMoveEventProducer = new MouseMoveEventProducer();
            mouseScrollEventProducer = new MouseScrollEventProducer();
            mouseModuleConfiguration = new TestMouseModuleConfiguration();

            // initialize the container and fulfill the MEF inports exports
            container = new CompositionContainer();
            container.ComposeExportedValue(mouseClickEventProducer);
            container.ComposeExportedValue(mouseMoveEventProducer);
            container.ComposeExportedValue(mouseScrollEventProducer);
            container.ComposeExportedValue(mouseModuleConfiguration);
            container.ComposeParts(mouseModule);

            //initialize the native mouse mock
            nativeMouseMock = new Mock<INativeMouse>();

            //initialzie the hookNativeMethodsMock
            hookNativeMethodsMock = new HookNativeMethodsMock();
            hookNativeMethodsMock.Initialize();
        }

        [TestMethod]
        public void TestMouseModule_ActivateTrue()
        {
            // Preconditions
            Debug.Assert(mouseModule != null);

            /* GIVEN */

            /* WHEN */
            mouseModule.Initialize(true);
            AllowMessageTypeRegistryForAll();
            hookNativeMethodsMock.AllowLibraryLoad();
            mouseModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(mouseModule.IsActive);
        }

        [TestMethod]
        public void TestMouseModule_ActivateFalse()
        {
            // Preconditions
            Debug.Assert(mouseModule != null);

            /* GIVEN */

            /* WHEN */
            mouseModule.Initialize(true);
            AllowMessageTypeRegistryForAll();
            hookNativeMethodsMock.AllowLibraryLoad();
            mouseModule.IsActive = true;
            mouseModule.IsActive = false;

            /* THEN */
            Assert.IsFalse(mouseModule.IsActive);
        }

        [TestMethod]
        public void TestMouseModule_InitializeFalse()
        {
            // Preconditions
            Debug.Assert(mouseModule != null);

            /* GIVEN */

            /* WHEN */
            mouseModule.Initialize(false);
            /* THEN */
            Assert.IsTrue(mouseClickEventProducer.IsClosed);
            Assert.IsTrue(mouseMoveEventProducer.IsClosed);
            Assert.IsTrue(mouseScrollEventProducer.IsClosed);
        }

        [TestMethod]
        public void TestMouseModuleInitializeTrue()
        {
            // Preconditions
            Debug.Assert(mouseModule != null);

            /* GIVEN */

            /* WHEN */
            mouseModule.Initialize(true);

            /* THEN */
            Assert.IsFalse(mouseClickEventProducer.IsClosed);
            Assert.IsFalse(mouseMoveEventProducer.IsClosed);
            Assert.IsFalse(mouseScrollEventProducer.IsClosed);
        }

        /// <summary>
        ///     Call AllowMessageTypeRegistry() methods on all messages related to the mouse producers.
        /// </summary>
        private void AllowMessageTypeRegistryForAll()
        {
            foreach (GlobalHook.MessageType messageType in mouseClickListenedMessagesTypes)
            {
                hookNativeMethodsMock.AllowMessageTypeRegistry(messageType);
            }
            foreach (GlobalHook.MessageType messageType in mouseScrollListenedMessagesTypes)
            {
                hookNativeMethodsMock.AllowMessageTypeRegistry(messageType);
            }
        }
    }
}