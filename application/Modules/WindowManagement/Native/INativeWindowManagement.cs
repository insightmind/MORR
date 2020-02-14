using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MORR.Modules.WindowManagement.Native
{
    public interface INativeWindowManagement
    {
        string GetWindowTitleFromHwnd(IntPtr hwnd);

        string GetProcessNameFromHwnd(IntPtr hwnd);

        bool IsRectSizeEqual(Rectangle rectA, Rectangle rectB);

        int GetWindowWidth(Rectangle rect);

        int GetWindowHeight(Rectangle rect);

        IntPtr GetForegroundWindow();

        long GetWindowRect(int hWnd, ref Rectangle lpRect);
    }
}
