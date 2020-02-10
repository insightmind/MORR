using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Shared.Hook;
using MORR.Shared.Hook.Exceptions;

namespace SharedTest.Hook
{
    [TestClass]
    public class GlobalHookTest
    {
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

            /* WHEN */

            /* THEN */
        }

        [TestMethod]
        public void TestGlobalHook_RemoveListener()
        {
            /* PRECONDITIONS */
            Debug.Assert(mockNativeHook != null);

            /* GIVEN */

            /* WHEN */

            /* THEN */
        }

        [TestMethod]
        public void TestGlobalHook_FreeLibrary()
        {
            /* PRECONDITIONS */
            Debug.Assert(mockNativeHook != null);

            /* GIVEN */

            /* WHEN */

            /* THEN */
        }
    }
}
