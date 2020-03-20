using System;
using MORR.Shared.Hook;

namespace MORR.Modules.Clipboard.Native
{
    public class ClipboardWindowMessageSink : IClipboardWindowMessageSink
    {
        private INativeClipboard.WindowProcedureHandler? internalWindowMessageHandler;

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


            NativeClipboard.RegisterClass(ref windowClass);

            // Creates window to register clipboard update messages
            WindowHandle = NativeClipboard.CreateWindowEx(0,
                                                          className,
                                                          "",
                                                          0,
                                                          0, 0,
                                                          1, 1,
                                                          IntPtr.Zero,
                                                          IntPtr.Zero,
                                                          IntPtr.Zero,
                                                          IntPtr.Zero);

            NativeClipboard.AddClipboardFormatListener(WindowHandle);
        }

        private static INativeClipboard NativeClipboard { get; } = new NativeClipboard();

        /// <summary>
        ///     The underlying window handle
        /// </summary>
        private IntPtr WindowHandle { get; }

        /// <summary>
        ///     Event invoked when clipboard is updated
        /// </summary>
        public event IClipboardWindowMessageSink.ClipboardEventHandler? ClipboardUpdated;

        IntPtr IClipboardWindowMessageSink.OnClipboardUpdate(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
        {
            return OnClipboardUpdate(hWnd, messageId, wParam, lParam);
        }

        private IntPtr OnClipboardUpdate(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
        {
            if (messageId == (int) GlobalHook.MessageType.WM_CLIPBOARDUPDATE)
            {
                ClipboardUpdated?.Invoke(hWnd, messageId, wParam, lParam);
            }

            return NativeClipboard.DefWindowProc(hWnd, messageId, wParam, lParam);
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
            NativeClipboard.RemoveClipboardFormatListener(WindowHandle);
            NativeClipboard.DestroyWindow(WindowHandle);
            internalWindowMessageHandler = null;
        }

        #endregion
    }
}