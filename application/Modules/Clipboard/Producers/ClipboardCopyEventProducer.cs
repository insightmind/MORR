using System;
using MORR.Modules.Clipboard.Events;
using MORR.Modules.Clipboard.Native;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;

namespace MORR.Modules.Clipboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ClipboardCopyEvent
    /// </summary>
    public class ClipboardCopyEventProducer : DefaultEventQueue<ClipboardCopyEvent>
    {
        private static readonly ClipboardWindowMessageSink clipboardWindowMessageSink = new ClipboardWindowMessageSink();

        #region Private methods

        private void OnClipboardUpdate(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
        {
            if (messageId != (int) GlobalHook.MessageType.WM_CLIPBOARDUPDATE)
            {
                return;
            }

            var text = ClipboardNativeMethods.GetClipboardText();

            var clipboardCopyEvent = new ClipboardCopyEvent
                { ClipboardText = text, IssuingModule = ClipboardModule.Identifier };
            Enqueue(clipboardCopyEvent);
        }

        #endregion

        #region Window

        /// <summary>
        ///     Creates a window procedure to receive window messages when clipboard is updated
        /// </summary>
        internal class ClipboardWindowMessageSink
        {
            /// <summary>
            ///     Handles a window message when clipboard is updated
            /// </summary>
            /// <param name="IntPtr">The pointer to the current window</param>
            /// <param name="messageId">The identifier of the message</param>
            /// <param name="wParam">The WPARAM of the message</param>
            /// <param name="lParam">The LPARAM of the message</param>
            public delegate void ClipboardEventHandler(IntPtr hwnd, uint uMsg, IntPtr wParam, IntPtr lParam);

            private ClipboardNativeMethods.WindowProcedureHandler internalWindowMessageHandler;

            public ClipboardWindowMessageSink()
            {
                var className = $"sink@{Guid.NewGuid()}";

                internalWindowMessageHandler = OnClipboardUpdate;

                ClipboardNativeMethods.WindowClass windowClass;

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


                ClipboardNativeMethods.RegisterClass(ref windowClass);

                // Creates window to register clipboard update messages
                WindowHandle = ClipboardNativeMethods.CreateWindowEx(0,
                                                            className,
                                                            "",
                                                            0,
                                                            0, 0,
                                                            1, 1,
                                                            IntPtr.Zero,
                                                            IntPtr.Zero,
                                                            IntPtr.Zero,
                                                            IntPtr.Zero);

                ClipboardNativeMethods.AddClipboardFormatListener(WindowHandle);
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

                return ClipboardNativeMethods.DefWindowProc(hWnd, messageId, wParam, lParam);
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
                ClipboardNativeMethods.RemoveClipboardFormatListener(WindowHandle);
                ClipboardNativeMethods.DestroyWindow(WindowHandle);
                internalWindowMessageHandler = null;
            }

            #endregion
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Sets the hook for the clipboard copy event.
        /// </summary>
        public void StartCapture()
        {
            if (clipboardWindowMessageSink == null)
            {
                return;
            }

            clipboardWindowMessageSink.ClipboardUpdated += OnClipboardUpdate;
        }

        /// <summary>
        ///     Releases the hook for the clipboard copy event.
        /// </summary>
        public void StopCapture()
        {
            clipboardWindowMessageSink?.Dispose();
            Close();
        }

        #endregion
    }
}