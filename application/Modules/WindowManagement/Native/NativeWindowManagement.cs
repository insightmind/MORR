using System;
using System.Diagnostics;
using Point = System.Windows.Point;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;

namespace MORR.Modules.WindowManagement.Native
{
    internal class NativeWindowManagement : INativeWindowManagement
    {
        /// <summary>
        ///     Get the title of a window from it's Hwnd
        /// </summary>
        /// <param name="hwnd"> Hwnd of the window</param>
        /// <returns>the title of the window in string</returns>
        public string GetWindowTitleFromHwnd(IntPtr hwnd)
        {
            var textLength = GetWindowTextLength(hwnd);
            var windowTextStringBuilder = new StringBuilder(textLength + 1);
            GetWindowText(hwnd, windowTextStringBuilder, windowTextStringBuilder.Capacity);
            return windowTextStringBuilder.ToString();
        }

        /// <summary>
        ///     Get the The name of the process associated with a window
        ///     from the window's Hwnd
        /// </summary>
        /// <param name="hwnd">Hwnd of the window</param>
        /// <returns>the name of the process associated with the window</returns>
        public string GetProcessNameFromHwnd(IntPtr hwnd)
        {
            GetWindowThreadProcessId(hwnd, out var pid);
            var process = Process.GetProcessById((int) pid);
            return process.ProcessName;
        }

        public Point GetPoint(int x, int y)
        {
            return new Point(x,y);
        }
        public bool IsRectSizeEqual(Rectangle rectA, Rectangle rectB)
        {
            return GetWindowWidth(rectA) == GetWindowWidth(rectB) && GetWindowHeight(rectA) == GetWindowHeight(rectB);
        }

        public int GetWindowWidth(Rectangle rect)
        {
            return rect.Width - rect.X;
        }

        public int GetWindowHeight(Rectangle rect)
        {
            return rect.Height - rect.Y;
        }

        IntPtr INativeWindowManagement.GetForegroundWindow()
        {
            return GetForegroundWindow();
        }

        long INativeWindowManagement.GetWindowRect(int hWnd, ref Rectangle lpRect)
        {
            return GetWindowRect(hWnd, ref lpRect);
        }

        #region DllImports

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern long GetWindowRect(int hWnd, ref Rectangle lpRect);

        #endregion
    }
}