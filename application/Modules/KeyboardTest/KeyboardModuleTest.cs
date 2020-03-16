using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.Keyboard;
using MORR.Modules.Keyboard.Native;
using MORR.Modules.Keyboard.Producers;
using MORR.Shared.Events.Queue;
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
        private Mock<INativeKeyboard> nativeKeyboardMock;
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

            //initialize the native keyboard mock
            nativeKeyboardMock = new Mock<INativeKeyboard>();

            //initialzie the hookNativeMethodsMock
            hookNativeMethodsMock = new HookNativeMethodsMock();
            hookNativeMethodsMock.Initialize();
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

        ///////////////////////helper methods///////////////////////////
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

        /// <summary>
        ///     Performs a series of initialization and Setups to get the CppGetMessageCallback.
        /// </summary>
        /// <returns>the callback that can be called with a message, which in turns calls a callback in the producers that is interested in this type of message</returns>
        private GlobalHook.CppGetMessageCallback GetCallback()
        {
            GlobalHook.CppGetMessageCallback callback = null;
            AllowMessageTypeRegistryForAll();
            hookNativeMethodsMock.AllowLibraryLoad();
            var callbackReceivedEvent = new AutoResetEvent(false);

            hookNativeMethodsMock.Mock
                 .Setup(hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()))?
                 .Callback((GlobalHook.CppGetMessageCallback cppCallback, bool isBlocking) =>
                 {
                     callback = cppCallback;
                     callbackReceivedEvent.Set();
                 });
            //here the SetHook() method is called!
            keyboardModule.Initialize(true);
            keyboardModule.IsActive = true;

            //wait for the hookNativeMethodsMock.Mock.Callback is called!
            Assert.IsTrue(callbackReceivedEvent.WaitOne(maxWaitTime), "Did not receive callback in time!");
            Assert.IsNotNull(callback, "Callback received however unexpectedly null!");
            return callback;
        }
    }
}