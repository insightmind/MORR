using System;
using System.Runtime.InteropServices;

namespace MORR.Core.Data.Capture.Video.Desktop.Utility
{
    public static class NativeDesktop
    {
        /// <summary>
        ///     Attempts to get a window associated with the current process.
        /// </summary>
        /// <returns>The handle of a window associated with the current process.</returns>
        public static IntPtr GetAssociatedWindow()
        {
            var activeWindow = GetActiveWindow();
            var consoleWindow = GetConsoleWindow();

            return activeWindow != IntPtr.Zero ? activeWindow : consoleWindow;
        }

        #region Methods

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        internal static extern IntPtr GetActiveWindow();

        #endregion
    }
}
