using Moq;
using MORR.Modules.Clipboard.Native;

namespace ClipboardTest
{
    public class NativeClipboardMock
    {
        public readonly Mock<INativeClipboard> Mock = new Mock<INativeClipboard>();

        public void GetText()
        {
            Mock.Setup(nativeCl => nativeCl.GetClipboardText()).Returns("ClipboardText");
        }
    }
}