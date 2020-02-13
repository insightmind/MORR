using System;
using System.Runtime.InteropServices;

namespace MORR.Core.Data.Capture.Video.Desktop.Utility
{
    public static class DesktopCaptureNativeMethods
    {
        #region Event handler

        public static Func<WindowHandleWrapper>? WindowRequestedHandler { get; set; }

        #endregion

        /// <summary>
        ///     Attempts to get a window associated with the current process.
        /// </summary>
        /// <returns>The wrapper around a handle of a window associated with the current process.</returns>
        public static WindowHandleWrapper GetAssociatedWindow()
        {
            var consoleWindow = GetConsoleWindow();

            if (consoleWindow != IntPtr.Zero)
            {
                return new WindowHandleWrapper(consoleWindow);
            }

            if (WindowRequestedHandler == null)
            {
                throw new InvalidOperationException(
                    "No window requested handler has been registered and no console window was found.");
            }

            return WindowRequestedHandler();
        }

        #region Methods

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetConsoleWindow();

        /// <summary>
        ///     Wraps a window handle with a cleanup callback.
        /// </summary>
        public class WindowHandleWrapper : IDisposable
        {
            private readonly Action? cleanupCallback;

            public WindowHandleWrapper(IntPtr handle, Action? cleanupCallback = null)
            {
                Handle = handle;
                this.cleanupCallback = cleanupCallback;
            }

            /// <summary>
            ///     The handle of the window.
            /// </summary>
            public IntPtr Handle { get; }

            public void Dispose()
            {
                cleanupCallback?.Invoke();
            }
        }

        #endregion
    }
}