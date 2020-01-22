using System;
using System.Composition;
using System.Runtime.InteropServices;
using MORR.Modules.Clipboard.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.Clipboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ClipboardCopyEvent
    /// </summary>
    [Export(typeof(ClipboardCopyEventProducer))]
    [Export(typeof(EventQueue<ClipboardCopyEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class ClipboardCopyEventProducer : EventQueue<ClipboardCopyEvent>
    {
        private static readonly ClipboardWindowMessageSink clipboardWindowMessageSink = new ClipboardWindowMessageSink();

        /// <summary>
        /// Creates a ClipboardCutEventProducer with bounded multiconsumer strategy
        /// </summary>
        public ClipboardCopyEventProducer() : base(new BoundedMultiConsumerChannelStrategy()) { }

        #region Public methods

        /// <summary>
        ///     Sets the hook for the clipboard copy event.
        /// </summary>
        public void HookClipboardCopyEvents()
        {
            if (clipboardWindowMessageSink != null)
            {
                clipboardWindowMessageSink.ClipboardUpdated += OnClipboardUpdate;
            }
        }

        /// <summary>
        ///     Releases the hook for the clipboard copy event.
        /// </summary>
        public void UnhookClipboardCopyEvents()
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
            if (messageId != NativeMethods.WM_CLIPBOARDUPDATE)
            {
                return;
            }

            var text = getTextClipboard(hWnd);


            var @event = new ClipboardCopyEvent() { Text = text };
            Enqueue(@event);
        }

        /// <summary>
        ///     Gets the text from the clipboard
        /// </summary>
        /// <param name="hwnd">Pointer to the window that currently has clipboard</param>
        /// <returns>String representing text from the clipboard</returns>
        private string getTextClipboard(IntPtr hwnd)
        {
            NativeMethods.OpenClipboard(hwnd);

            //Get pointer to clipboard data in the selected format
            IntPtr ClipboardDataPointer = NativeMethods.GetClipboardData(NativeMethods.CF_TEXT);

            // Lock the handle to get the actual text pointer
            UIntPtr Length = NativeMethods.GlobalSize(ClipboardDataPointer);
            IntPtr gLock = NativeMethods.GlobalLock(ClipboardDataPointer);

            string text;

            text = Marshal.PtrToStringAuto(gLock);

            // Release the lock
            NativeMethods.GlobalUnlock(gLock);

            // Release the clipboard
            NativeMethods.CloseClipboard();

            return text;
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

            private NativeMethods.WindowProcedureHandler internalWindowMessageHandler;

            public ClipboardWindowMessageSink()
            {
                var className = $"sink@{Guid.NewGuid()}";

                internalWindowMessageHandler = OnClipboardUpdate;

                NativeMethods.WindowClass windowClass;

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


                NativeMethods.RegisterClass(ref windowClass);

                // Creates window to register clipboard update messages
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
                
                NativeMethods.AddClipboardFormatListener(WindowHandle);
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
                if (messageId == NativeMethods.WM_CLIPBOARDUPDATE)
                {
                    ClipboardUpdated?.Invoke(hWnd, messageId, wParam, lParam);
                }

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
                NativeMethods.RemoveClipboardFormatListener(WindowHandle);
                NativeMethods.DestroyWindow(WindowHandle);
                internalWindowMessageHandler = null;
            }

            #endregion
        }

        #endregion

        #region Native methods

        internal static class NativeMethods
        {
            // Window message value
            public const int WM_CLIPBOARDUPDATE = 0x031D;

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

            /// <summary>
            /// Opens clipboard
            /// </summary>
            /// <param name="hWndNewOwner">Pointer to the window that currently has clipboard </param>
            /// <returns>true on success false otherwise</returns> 
            [DllImport("user32.dll")]
            internal static extern bool OpenClipboard(IntPtr hWndNewOwner);
            /// <summary>
            /// Closes clipboard
            /// </summary>
            /// <returns>true on success false otherwise</returns>
            [DllImport("user32.dll")]
            internal static extern bool CloseClipboard();
            /// <summary>
            /// Gets pointer to the window that currently has clipboard
            /// </summary>
            /// <returns></returns>

            /// <summary>
            /// Gets data of the clipboard
            /// </summary>
            /// <param name="uFormat">Format of clipboard data</param>
            /// <returns>Pointer to clipboard data</returns>
            [DllImport("user32.dll")]
            internal static extern IntPtr GetClipboardData(uint uFormat);

            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GlobalUnlock(IntPtr hMem);

            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern IntPtr GlobalLock(IntPtr hMem);

            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern UIntPtr GlobalSize(IntPtr hMem);


            #endregion
        }

        #endregion
    }
}
