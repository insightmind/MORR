using System;
using System.Collections.Generic;
using Point = System.Windows.Point;
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

        Point GetPoint(int x, int y);

        IntPtr GetForegroundWindow();

        long GetWindowRect(int hWnd, ref Rectangle lpRect);
    }
}
