using System;

namespace Morr.Core.UI.Controls.NotifyIcon.Utility
{
    /// <summary>
    /// Creates a window procedure to receive window and taskbar messages
    /// </summary>
    public class WindowMessageSink : IDisposable
    {
        /// <summary>
        /// Handles the taskbar created event
        /// </summary>
        public delegate void TaskbarCreatedEventHandler();

        /// <summary>
        /// Handles a window message
        /// </summary>
        /// <param name="messageId">The identifier of the message</param>
        /// <param name="wParam">The WPARAM of the message</param>
        /// <param name="lParam">The LPARAM of the message</param>
        public delegate void WindowMessageEventHandler(uint messageId, IntPtr wParam, IntPtr lParam);

        private readonly uint taskbarWindowMessageId;

        public WindowMessageSink()
        {
            var className = $"sink@{Guid.NewGuid()}";

            NativeMethods.WindowClass windowClass;

            windowClass.style = 0;
            windowClass.lpfnWndProc = OnWindowMessageInternal;
            windowClass.cbClsExtra = 0;
            windowClass.cbWndExtra = 0;
            windowClass.hInstance = IntPtr.Zero;
            windowClass.hIcon = IntPtr.Zero;
            windowClass.hCursor = IntPtr.Zero;
            windowClass.hbrBackground = IntPtr.Zero;
            windowClass.lpszMenuName = "";
            windowClass.lpszClassName = className;

            NativeMethods.RegisterClass(ref windowClass);
            taskbarWindowMessageId = NativeMethods.RegisterWindowMessage("TaskbarCreated");

            WindowHandle = NativeMethods.CreateWindowEx(0,
                                                        className,
                                                        "",
                                                        0,
                                                        0, 0,
                                                        1, 1,
                                                        IntPtr.Zero,
                                                        IntPtr.Zero,
                                                        IntPtr.Zero,
                                                        IntPtr.Zero);
        }

        /// <summary>
        /// The underlying window handle
        /// </summary>
        public IntPtr WindowHandle { get; }

        /// <summary>
        /// Event invoked when a window message is received
        /// </summary>
        public event WindowMessageEventHandler? WindowMessage;

        /// <summary>
        /// Event invoked when the taskbar created message is received
        /// </summary>
        public event TaskbarCreatedEventHandler? TaskbarCreated;

        private IntPtr OnWindowMessageInternal(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
        {
            if (messageId == taskbarWindowMessageId)
            {
                TaskbarCreated?.Invoke();
            }

            WindowMessage?.Invoke(messageId, wParam, lParam);

            return NativeMethods.DefWindowProc(hWnd, messageId, wParam, lParam);
        }

        #region Dispose

        private bool isDisposed;

        /// <summary>
        /// Frees all unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        ~WindowMessageSink()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (isDisposed || !disposing)
            {
                return;
            }

            isDisposed = true;

            NativeMethods.DestroyWindow(WindowHandle);
        }

        #endregion
    }
}