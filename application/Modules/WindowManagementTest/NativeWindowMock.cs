using System;
using System.Drawing;
using Moq;
using MORR.Modules.WindowManagement.Native;
using Point = System.Windows.Point;

namespace WindowManagementTest
{
    public class NativeWindowMock
    {
        public readonly Mock<INativeWindowManagement> Mock = new Mock<INativeWindowManagement>();
        private Rectangle rect;

        public void GetForegroundWindow()
        {
            Mock.Setup(nativeWin => nativeWin.GetForegroundWindow()).Returns((IntPtr) 1);
        }

        public void GetProcessName()
        {
            Mock.Setup(nativeWin => nativeWin.GetProcessNameFromHwnd((IntPtr) 1))
                .Returns("ProcessName");
        }

        public void GetPoint()
        {
            Mock.SetupSequence(nativeWin => nativeWin.GetPoint(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new Point(0, 0)).Returns(new Point(1, 1));
        }

        public void GetTitle()
        {
            Mock.Setup(nativeWin => nativeWin.GetWindowTitleFromHwnd((IntPtr) 1)).Returns("Title");
        }


        public void GetWindowRect()
        {
            Mock.SetupSequence(nativeWin => nativeWin.GetWindowRect(1, ref rect)).Returns(0).Returns(2).Returns(0);
        }

        public void IsRectSizeEqual()
        {
            Mock.Setup(nativeWin => nativeWin.IsRectSizeEqual(It.IsAny<Rectangle>(), It.IsAny<Rectangle>()))
                .Returns(true);
        }

        public void IsRectSizeNotEqual()
        {
            Mock.Setup(nativeWin => nativeWin.IsRectSizeEqual(It.IsAny<Rectangle>(), It.IsAny<Rectangle>()))
                .Returns(false);
        }

        private void GetHeight()
        {
            Mock.SetupSequence(nativeWin => nativeWin.GetWindowHeight(It.IsAny<Rectangle>())).Returns(1).Returns(2);
        }


        private void GetWidth()
        {
            Mock.SetupSequence(nativeWin => nativeWin.GetWindowWidth(It.IsAny<Rectangle>())).Returns(1).Returns(2);
        }


        public void GetWidthAndHeight()
        {
            GetHeight();
            GetWidth();
        }
    }
}