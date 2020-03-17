using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Modules.Keyboard;
using MORR.Modules.Keyboard.Producers;
using MORR.Shared.Hook;
using SharedTest.TestHelpers.INativeHook;

namespace KeyboardTest
{
    [TestClass]
    public class KeyboardModuleTest
    {
        protected const int maxWaitTime = 5000;

        private CompositionContainer container;
        private KeyboardInteractEventProducer keyboardInteractEventProducer;
        private KeyboardModule keyboardModule;
        private HookNativeMethodsMock hookNativeMethodsMock;

        private readonly GlobalHook.MessageType[] KeyboardInteractListenedMessagesTypes =
        {
            GlobalHook.MessageType.WM_KEYDOWN,
            GlobalHook.MessageType.WM_SYSKEYDOWN
        };

        [TestInitialize]
        public void BeforeTest()
        {
            //initialize module, producers and configuration
            keyboardModule = new KeyboardModule();
            keyboardInteractEventProducer = new KeyboardInteractEventProducer();

            // initialize the container and fulfill the MEF inports exports
            container = new CompositionContainer();
            container.ComposeExportedValue(keyboardInteractEventProducer);
            container.ComposeParts(keyboardModule);

            //initialzie the hookNativeMethodsMock
            hookNativeMethodsMock = new HookNativeMethodsMock();
            hookNativeMethodsMock.Initialize();
        }

        [TestCleanup]
        public void AfterTest()
        {
            // null everything!
            keyboardModule = null;
            keyboardInteractEventProducer = null;
            container.Dispose();
            container = null;
            hookNativeMethodsMock = null;
        }

        [TestMethod]
        public void TestKeyboardModule_ActivateTrue()
        {
            // Preconditions
            Debug.Assert(keyboardModule != null);

            /* GIVEN */

            /* WHEN */
            keyboardModule.Initialize(true);
            AllowMessageTypeRegistryForAll();
            hookNativeMethodsMock.AllowLibraryLoad();
            keyboardModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(keyboardModule.IsActive);
        }

        [TestMethod]
        public void TestKeyboardModule_ActivateFalse()
        {
            // Preconditions
            Debug.Assert(keyboardModule != null);

            /* GIVEN */

            /* WHEN */
            keyboardModule.Initialize(true);
            AllowMessageTypeRegistryForAll();
            hookNativeMethodsMock.AllowLibraryLoad();
            keyboardModule.IsActive = true;
            keyboardModule.IsActive = false;

            /* THEN */
            Assert.IsFalse(keyboardModule.IsActive);
        }

        [TestMethod]
        public void TestKeyboardModule_InitializeFalse()
        {
            // Preconditions
            Debug.Assert(keyboardModule != null);

            /* GIVEN */

            /* WHEN */
            keyboardModule.Initialize(false);
            /* THEN */
            Assert.IsTrue(keyboardInteractEventProducer.IsClosed);
        }

        [TestMethod]
        public void TestKeyboardModuleInitializeTrue()
        {
            // Preconditions
            Debug.Assert(keyboardModule != null);

            /* GIVEN */

            /* WHEN */
            keyboardModule.Initialize(true);

            /* THEN */
            Assert.IsFalse(keyboardInteractEventProducer.IsClosed);
        }

        /// <summary>
        ///     Call AllowMessageTypeRegistry() methods on all messages related to keyboard producers.
        /// </summary>
        private void AllowMessageTypeRegistryForAll()
        {
            foreach (GlobalHook.MessageType messageType in KeyboardInteractListenedMessagesTypes)
            {
                hookNativeMethodsMock.AllowMessageTypeRegistry(messageType);
            }
        }
    }
}