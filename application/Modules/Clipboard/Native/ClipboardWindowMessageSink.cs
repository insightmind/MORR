using System;
using MORR.Shared.Hook;

namespace MORR.Modules.Clipboard.Native
{
    public class ClipboardWindowMessageSink
    {
        private static INativeClipboard nativeClipboard = new NativeClipboard();

        /// <summary>
        ///     Handles a window message when clipboard is updated
        /// </summary>
        /// <param name="IntPtr">The pointer to the current window</param>
        /// <param name="messageId">The identifier of the message</param>
        /// <param name="wParam">The WPARAM of the message</param>
        /// <param name="lParam">The LPARAM of the message</param>
        public delegate void ClipboardEventHandler(IntPtr hwnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        public INativeClipboard NativeClipboard
        {
            get { return nativeClipboard; }
            private set { }
        }


        private INativeClipboard.WindowProcedureHandler internalWindowMessageHandler;

        public ClipboardWindowMessageSink()
        {
            var className = $"sink@{Guid.NewGuid()}";

            internalWindowMessageHandler = OnClipboardUpdate;

            INativeClipboard.WindowClass windowClass;

            windowClass.style = 0;
            windowClass.lpfnWndProc = internalWindowMessageHandler;
            windowClass.cbClsExtra = 0;
            windowClass.cbWndExtra = 0;
            windowClass.hInstance = IntPtr.Zero;
            windowClass.hIcon = IntPtr.Zero;
            windowClass.hCursor = IntPtr.Zero;
            windowClass.hbrBackground = IntPtr.Zero;
            windowClass.lpszMenuName = "";
            windowClass.lpszClassName = className;


            nativeClipboard.RegisterClass(ref windowClass);

            // Creates window to register clipboard update messages
            WindowHandle = nativeClipboard.CreateWindowEx(0,
                                                          className,
                                                          "",
                                                          0,
                                                          0, 0,
                                                          1, 1,
                                                          IntPtr.Zero,
                                                          IntPtr.Zero,
                                                          IntPtr.Zero,
                                                          IntPtr.Zero);

            nativeClipboard.AddClipboardFormatListener(WindowHandle);
        }

        /// <summary>
        ///     The underlying window handle
        /// </summary>
        private IntPtr WindowHandle { get; }

        /// <summary>
        ///     Event invoked when clipboard is updated
        /// </summary>
        public event ClipboardEventHandler? ClipboardUpdated;

        private IntPtr OnClipboardUpdate(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
        {
            if (messageId == (int) GlobalHook.MessageType.WM_CLIPBOARDUPDATE)
            {
                ClipboardUpdated?.Invoke(hWnd, messageId, wParam, lParam);
            }

            return nativeClipboard.DefWindowProc(hWnd, messageId, wParam, lParam);
        }

        #region Dispose

        private bool isDisposed;

        /// <summary>
        ///     Frees all unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        ~ClipboardWindowMessageSink()
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
            nativeClipboard.RemoveClipboardFormatListener(WindowHandle);
            nativeClipboard.DestroyWindow(WindowHandle);
            internalWindowMessageHandler = null;
        }

        #endregion
    }
}


    
