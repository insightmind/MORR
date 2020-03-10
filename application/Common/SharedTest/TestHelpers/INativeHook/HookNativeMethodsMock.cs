using System;
using System.Diagnostics;
using Moq;
using MORR.Shared.Hook;

namespace SharedTest.TestHelpers.INativeHook
{
    public class HookNativeMethodsMock
    {
        public readonly Mock<IHookNativeMethods> Mock = new Mock<IHookNativeMethods>();
        public const string mockLibraryName = "MockLibraryName.someDLL";
        public readonly IntPtr mockLibraryHandle = new IntPtr(0x1);

        public void Initialize()
        {
            Debug.Assert(Mock != null);

            Mock
                .SetupGet(mock => mock.HookLibraryName)?
                .Returns(mockLibraryName);

            GlobalHook.Initialize(Mock.Object);
        }

        public void AllowLibraryLoad()
        {
            Mock?
                .Setup(mock => mock.LoadLibrary())?
                .Returns(mockLibraryHandle); // We just return a non null pointer
        }

        public void DisallowLibraryLoad()
        {
            Mock?
                .Setup(mock => mock.LoadLibrary())?
                .Returns(new IntPtr(0x0)); // We just return a non null pointer
        }

        public void AllowMessageTypeRegistry(GlobalHook.MessageType messageType)
        {
            Mock?
                .Setup(hook => hook.Capture((uint)messageType))?
                .Returns(true);
        }

        public void DisallowMessageTypeRegistry(GlobalHook.MessageType messageType)
        {
            Mock?
                .Setup(hook => hook.Capture((uint)messageType))?
                .Returns(false);
        }
    }
}
