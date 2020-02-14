using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace MORR.Modules.WindowManagement.Native
{
    internal sealed class WindowManagementNativeMethods
    {
        /// <summary>
        ///     Get the title of a window from it's Hwnd
        /// </summary>
        /// <param name="hwnd"> Hwnd of the window</param>
        /// <returns>the title of the window in string</returns>
        public static string GetWindowTitleFromHwnd(IntPtr hwnd)
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
        public static string GetProcessNameFromHwnd(IntPtr hwnd)
        {
            GetWindowThreadProcessId(hwnd, out var pid);
            var process = Process.GetProcessById((int)pid);
            return process.ProcessName;
        }

        public static bool IsRectSizeEqual(Rectangle rectA, Rectangle rectB)
        {
            return GetWindowWidth(rectA) == GetWindowWidth(rectB) && GetWindowHeight(rectA) == GetWindowHeight(rectB);
        }

        public static int GetWindowWidth(Rectangle rect)
        {
            return rect.Width - rect.X;
        }

        public static int GetWindowHeight(Rectangle rect)
        {
            return rect.Height - rect.Y;
        }

        #region Window for process helper

        #endregion

        #region DllImports
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern long GetWindowRect(int hWnd, ref Rectangle lpRect);

        #endregion
    }
}
