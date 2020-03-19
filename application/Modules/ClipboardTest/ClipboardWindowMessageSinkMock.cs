using System;
using Moq;
using MORR.Modules.Clipboard.Native;
using MORR.Shared.Hook;

namespace ClipboardTest
{
    public class ClipboardWindowMessageSinkMock
    {
        public readonly Mock<IClipboardWindowMessageSink> Mock = new Mock<IClipboardWindowMessageSink>();


        public void Dispose()
        {
            Mock.Setup(msgSink => msgSink.Dispose());
        }

        public void ClipboardUpdateCopy()
        {
            Mock.Raise(mock => mock.ClipboardUpdated += null, (IntPtr) 1,
                       (uint) GlobalHook.MessageType.WM_CLIPBOARDUPDATE, (IntPtr) 7, IntPtr.Zero);
        }

        public void ClipboardUpdateCut()
        {
            Mock.Raise(mock => mock.ClipboardUpdated += null, (IntPtr) 1,
                       (uint) GlobalHook.MessageType.WM_CLIPBOARDUPDATE, (IntPtr) 14, IntPtr.Zero);
        }
    }
}