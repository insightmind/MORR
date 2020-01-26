using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using MORR.Modules.Clipboard.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.Clipboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ClipboardCopyEvent
    /// </summary>
    [Export(typeof(ClipboardCopyEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<ClipboardCopyEvent>))]
    [Export(typeof(IReadWriteEventQueue<ClipboardCopyEvent>))]
    [Export(typeof(IReadOnlyEventQueue<Event>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ClipboardCopyEventProducer : DefaultEventQueue<ClipboardCopyEvent>
    {
        private static readonly ClipboardWindowMessageSink clipboardWindowMessageSink = new ClipboardWindowMessageSink();


        #region Public methods

        /// <summary>
        ///     Sets the hook for the clipboard copy event.
        /// </summary>
        public void StartCapture()
        {
            if (clipboardWindowMessageSink != null)
            {
                clipboardWindowMessageSink.ClipboardUpdated += OnClipboardUpdate;
            }
        }

        /// <summary>
        ///     Releases the hook for the clipboard copy event.
        /// </summary>
        public void StopCapture()
        {
            if (clipboardWindowMessageSink != null)
            {
                clipboardWindowMessageSink.Dispose();
            }
        }


        #endregion

        #region Private methods

        private void OnClipboardUpdate(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
        {
            if (messageId != (int) NativeMethods.MessageType.WM_CLIPBOARDUPDATE)
            {
                return;
            }

            var text = NativeMethods.getClipboardText();

            var clipboardCopyEvent = new ClipboardCopyEvent { Text = text };
            Enqueue(clipboardCopyEvent);
        }


        #endregion

        #region Window

        /// <summary>
        /// Creates a window procedure to receive window messages when clipboard is updated
        /// </summary>
        internal class ClipboardWindowMessageSink
        {
            /// <summary>
            /// Handles a window message when clipboard is updated
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
            /// The underlying window handle
            /// </summary>
            private IntPtr WindowHandle { get; }

            /// <summary>
            /// Event invoked when clipboard is updated
            /// </summary>
            public event ClipboardEventHandler? ClipboardUpdated;

            private IntPtr OnClipboardUpdate(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
            {
                if (messageId == (int) NativeMethods.MessageType.WM_CLIPBOARDUPDATE)
                {
                    ClipboardUpdated?.Invoke(hWnd, messageId, wParam, lParam);
                }

                return ClipboardNativeMethods.DefWindowProc(hWnd, messageId, wParam, lParam);
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

        #region Clipboard native methods

        internal static class ClipboardNativeMethods
        {
            // Clipboard data format
            public const int CF_TEXT = 1;

            #region structs

            [StructLayout(LayoutKind.Sequential)]
            public struct WindowClass
            {
                public uint style;
                public WindowProcedureHandler lpfnWndProc;
                public int cbClsExtra;
                public int cbWndExtra;
                public IntPtr hInstance;
                public IntPtr hIcon;
                public IntPtr hCursor;
                public IntPtr hbrBackground;

                [MarshalAs(UnmanagedType.LPWStr)] public string lpszMenuName;

                [MarshalAs(UnmanagedType.LPWStr)] public string lpszClassName;
            }

            #endregion

            #region methods

            public delegate IntPtr WindowProcedureHandler(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", EntryPoint = "CreateWindowExW")]
            public static extern IntPtr CreateWindowEx(int dwExStyle,
                                                       [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
                                                       [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
                                                       int dwStyle,
                                                       int x,
                                                       int y,
                                                       int nWidth,
                                                       int nHeight,
                                                       IntPtr hWndParent,
                                                       IntPtr hMenu,
                                                       IntPtr hInstance,
                                                       IntPtr lpParam);

            [DllImport("user32.dll", EntryPoint = "RegisterClassW")]
            public static extern short RegisterClass(ref WindowClass lpWndClass);


            [DllImport("user32.dll")]
            public static extern bool DestroyWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);


            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AddClipboardFormatListener(IntPtr hwnd);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

            #endregion
        }

        #endregion
    }
}
