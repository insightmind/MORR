using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Shared.Hook;
using MORR.Shared.Hook.Exceptions;

namespace SharedTest.Hook
{
    [TestClass]
    public class GlobalHookTest
    {
        protected const int maxWaitTime = 500;
        private Mock<IHookNativeMethods> mockNativeHook;
        private const string mockLibraryName = "MockLibraryName.someDLL";

        [TestInitialize]
        public void BeforeTest()
        {
            mockNativeHook = new Mock<IHookNativeMethods>();
            mockNativeHook
                .SetupGet(mock => mock.HookLibraryName)?
                .Returns(mockLibraryName);

            GlobalHook.Initialize(mockNativeHook.Object);
        }

        [TestMethod]
        public void TestGlobalHook_IsActiveTrue_Success()
        {
            /* PRECONDITIONS */
            Debug.Assert(mockNativeHook != null);

            /* GIVEN */
            mockNativeHook
                .Setup(mock => mock.LoadLibrary())?
                .Returns(new IntPtr(0x1)); // We just return a non null pointer

            /* WHEN */
            GlobalHook.IsActive = true;
            GlobalHook.IsActive = true; // This should not call LoadLibrary Twice.

            /* THEN */
            Assert.IsTrue(GlobalHook.IsActive);

            mockNativeHook.Verify(mock => mock.LoadLibrary(), Times.Once);
            mockNativeHook.Verify(mock => mock.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), false), Times.Once);
        }

        [TestMethod]
        public void TestGlobalHook_IsActiveTrue_ErrorLoadingLibrary()
        {
            /* PRECONDITIONS */
            Debug.Assert(mockNativeHook != null);

            /* GIVEN */
            mockNativeHook
                .Setup(mock => mock.LoadLibrary())?
                .Returns(new IntPtr(0x0)); // We just return a zero pointer so it tells the hook it could not load library.

            /* WHEN */
            Assert.ThrowsException<HookLibraryException>(() => GlobalHook.IsActive = true);

            /* THEN */
            Assert.IsFalse(GlobalHook.IsActive);

            mockNativeHook.Verify(mock => mock.LoadLibrary(), Times.Once);
            mockNativeHook.Verify(mock => mock.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), false), Times.Never);
        }

        [TestMethod]
        public void TestGlobalHook_IsActiveFalse()
        {
            /* PRECONDITIONS */
            Debug.Assert(mockNativeHook != null);

            /* GIVEN */
            var pointer = new IntPtr(0x1);

            mockNativeHook
                .Setup(mock => mock.LoadLibrary())?
                .Returns(pointer); // We just return a non null pointer

            GlobalHook.IsActive = true;

            /* WHEN */
            GlobalHook.IsActive = false;
            GlobalHook.IsActive = false; // This should not call RemoveHook Twice.

            /* THEN */
            Assert.IsFalse(GlobalHook.IsActive);

            mockNativeHook.Verify(mock => mock.RemoveHook(), Times.Once);
        }

        [TestMethod]
        public void TestGlobalHook_AddListener()
        {
            /* PRECONDITIONS */
            Debug.Assert(mockNativeHook != null);

            /* GIVEN */
            const int numOfEvent = 50;
            const int numOfNotEvent = 20;
            const GlobalHook.MessageType messageType = GlobalHook.MessageType.WM_USER;

            var resetCounter = new CountdownEvent(numOfEvent);
            var autoReset = new AutoResetEvent(false);

            GlobalHook.CppGetMessageCallback callback = null;
            
            // Allow message type
            mockNativeHook
                .Setup(hook => hook.Capture((uint) messageType))?
                .Returns(true);

            // Allow loading library
            mockNativeHook
                .Setup(mock => mock.LoadLibrary())?
                .Returns(new IntPtr(0x1)); // We just return a non null pointer

            mockNativeHook
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
            Debug.Assert(mockNativeHook != null);

            /* GIVEN */
            const GlobalHook.MessageType messageType = GlobalHook.MessageType.WM_USER;
            mockNativeHook
                .Setup(hook => hook.Capture(It.IsAny<uint>()))?
                .Returns(false);

            /* WHEN */
            Assert.ThrowsException<NotSupportedException>(() => GlobalHook.AddListener(message => { }, messageType));

            /* THEN */
            mockNativeHook.Verify(hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void TestGlobalHook_RemoveListener()
        {
            /* PRECONDITIONS */
            Debug.Assert(mockNativeHook != null);

            /* GIVEN */
            const int numOfEvent = 50;
            const GlobalHook.MessageType messageType = GlobalHook.MessageType.WM_USER;

            var autoReset = new AutoResetEvent(false);

            GlobalHook.CppGetMessageCallback callback = null;

            // Allow message type
            mockNativeHook
                .Setup(hook => hook.Capture((uint)messageType))?
                .Returns(true);

            // Allow loading library
            mockNativeHook
                .Setup(mock => mock.LoadLibrary())?
                .Returns(new IntPtr(0x1)); // We just return a non null pointer

            mockNativeHook
                .Setup(hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()))?
                .Callback((GlobalHook.CppGetMessageCallback cppCallback, bool isBlocking) =>
                {
                    callback = cppCallback;
                    autoReset.Set();
                });

            /* WHEN */
            void Listener(GlobalHook.HookMessage message)
            {
                /* THEN */
                Assert.Fail("The listener should not get called!");
            }

            GlobalHook.AddListener(Listener, messageType);

            GlobalHook.IsActive = true;

            GlobalHook.RemoveListener(Listener, messageType);

            Assert.IsTrue(autoReset.WaitOne(maxWaitTime));
            Assert.IsNotNull(callback);

            for (var index = 0; index < numOfEvent; index++)
            {
                callback(new GlobalHook.HookMessage()
                {
                    Type = (uint)messageType
                });
            }
        }

        [TestMethod]
        public void TestGlobalHook_FreeLibrary()
        {
            /* PRECONDITIONS */
            Debug.Assert(mockNativeHook != null);

            /* GIVEN */

            // Allow loading library
            var pointer = new IntPtr(0x1);

            mockNativeHook
                .Setup(mock => mock.LoadLibrary())?
                .Returns(pointer); // We just return a non null pointer

            GlobalHook.IsActive = true;


            /* WHEN */
            GlobalHook.FreeLibrary();

            /* THEN */
            Assert.IsFalse(GlobalHook.IsActive);
            mockNativeHook.Verify(mock => mock.FreeLibrary(pointer), Times.AtLeastOnce);
        }
    }
}
