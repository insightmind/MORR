using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Shared.Hook;
using MORR.Shared.Hook.Exceptions;
using SharedTest.TestHelpers.INativeHook;

namespace SharedTest.Hook
{
    [TestClass]
    public class GlobalHookTest
    {
        protected const int maxWaitTime = 500;
        private HookNativeMethodsMock hookNativeMethods;

        [TestInitialize]
        public void BeforeTest()
        {
            hookNativeMethods = new HookNativeMethodsMock();
            hookNativeMethods.Initialize();
        }

        [TestMethod]
        public void TestGlobalHook_IsActiveTrue_Success()
        {
            /* PRECONDITIONS */
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            hookNativeMethods.AllowLibraryLoad();

            /* WHEN */
            GlobalHook.IsActive = true;
            GlobalHook.IsActive = true; // This should not call LoadLibrary Twice.

            /* THEN */
            Assert.IsTrue(GlobalHook.IsActive);

            hookNativeMethods.Mock.Verify(mock => mock.LoadLibrary(), Times.Once);
            hookNativeMethods.Mock.Verify(mock => mock.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), false), Times.Once);
        }

        [TestMethod]
        public void TestGlobalHook_IsActiveTrue_NullNativeHook()
        {
            /* PRECONDITIONS */

            /* GIVEN */
            GlobalHook.Initialize(null);

            /* WHEN */
            GlobalHook.IsActive = true;

            /* THEN */
            Assert.IsFalse(GlobalHook.IsActive);
        }

        [TestMethod]
        public void TestGlobalHook_IsActiveTrue_ErrorLoadingLibrary()
        {
            /* PRECONDITIONS */
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            hookNativeMethods.DisallowLibraryLoad();

            /* WHEN */
            Assert.ThrowsException<HookLibraryException>(() => GlobalHook.IsActive = true);

            /* THEN */
            Assert.IsFalse(GlobalHook.IsActive);

            hookNativeMethods.Mock.Verify(mock => mock.LoadLibrary(), Times.Once);
            hookNativeMethods.Mock.Verify(mock => mock.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), false), Times.Never);
        }

        [TestMethod]
        public void TestGlobalHook_IsActiveFalse()
        {
            /* PRECONDITIONS */
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            hookNativeMethods.AllowLibraryLoad();

            GlobalHook.IsActive = true;

            /* WHEN */
            GlobalHook.IsActive = false;
            GlobalHook.IsActive = false; // This should not call RemoveHook Twice.

            /* THEN */
            Assert.IsFalse(GlobalHook.IsActive);

            hookNativeMethods.Mock.Verify(mock => mock.RemoveHook(), Times.Once);
        }

        [TestMethod]
        public void TestGlobalHook_AddListener()
        {
            /* PRECONDITIONS */
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            const int numOfEvent = 50;
            const int numOfNotEvent = 20;
            const GlobalHook.MessageType messageType = GlobalHook.MessageType.WM_USER;

            using var resetCounter = new CountdownEvent(numOfEvent);
            using var autoReset = new AutoResetEvent(false);

            GlobalHook.CppGetMessageCallback callback = null;
            
            hookNativeMethods.AllowMessageTypeRegistry(messageType);
            hookNativeMethods.AllowLibraryLoad();
            hookNativeMethods.Mock
                 .Setup(hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()))?
                 .Callback((GlobalHook.CppGetMessageCallback cppCallback, bool isBlocking) =>
                 {
                     callback = cppCallback;
                     autoReset.Set();
                 });

            /* WHEN */
            GlobalHook.AddListener(message =>
            {
                Assert.AreEqual((uint) messageType, message.Type);
                resetCounter.Signal();
            }, messageType);

            GlobalHook.IsActive = true;

            Assert.IsTrue(autoReset.WaitOne(maxWaitTime));
            Assert.IsNotNull(callback);

            for (var index = 0; index < numOfEvent; index++)
            {
                callback(new GlobalHook.HookMessage()
                {
                    Type = (uint) messageType
                });
            }

            // We check if our listener still gets called on events we don't care about. If it will this 
            // test will throw because the resetCounter will be set to a negative number.
            for (var index = 0; index < numOfNotEvent; index++)
            {
                callback(new GlobalHook.HookMessage()
                {
                    Type = (uint) GlobalHook.MessageType.WM_APP
                });
            }

            /* THEN */
            Assert.IsTrue(resetCounter.Wait(maxWaitTime));
        }

        [TestMethod]
        public void TestGlobalHook_AddListener_UnsupportedMessageType()
        {
            /* PRECONDITIONS */
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            const GlobalHook.MessageType messageType = GlobalHook.MessageType.WM_USER;
            hookNativeMethods.DisallowMessageTypeRegistry(messageType);

            /* WHEN */
            Assert.ThrowsException<NotSupportedException>(() => GlobalHook.AddListener(message => { }, messageType));

            /* THEN */
            hookNativeMethods.Mock.Verify(hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void TestGlobalHook_RemoveListener()
        {
            /* PRECONDITIONS */
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            const GlobalHook.MessageType messageType = GlobalHook.MessageType.WM_USER;

            using var autoReset = new AutoResetEvent(false);

            GlobalHook.CppGetMessageCallback callback = null;

            hookNativeMethods.AllowMessageTypeRegistry(messageType);
            hookNativeMethods.AllowLibraryLoad();

            hookNativeMethods.Mock
                .Setup(hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()))?
                .Callback((GlobalHook.CppGetMessageCallback cppCallback, bool isBlocking) =>
                {
                    callback = cppCallback;
                    autoReset.Set();
                });

            /* WHEN */
            static void Listener(GlobalHook.HookMessage message)
            {
                /* THEN */
                Assert.Fail("The listener should not get called!");
            }

            GlobalHook.AddListener(Listener, messageType);

            GlobalHook.IsActive = true;

            GlobalHook.RemoveListener(Listener, messageType);

            Assert.IsTrue(autoReset.WaitOne(maxWaitTime));
            Assert.IsNotNull(callback);
        }

        [TestMethod]
        public void TestGlobalHook_FreeLibrary()
        {
            /* PRECONDITIONS */
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            hookNativeMethods.AllowLibraryLoad();
            GlobalHook.IsActive = true;

            /* WHEN */
            GlobalHook.FreeLibrary();

            /* THEN */
            Assert.IsFalse(GlobalHook.IsActive);
            hookNativeMethods.Mock.Verify(mockedObject => mockedObject.FreeLibrary(hookNativeMethods.MockLibraryHandle), Times.AtLeastOnce);
        }
    }
}
