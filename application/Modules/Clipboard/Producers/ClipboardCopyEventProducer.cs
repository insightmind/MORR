using System;
using System.Composition;
using System.Runtime.InteropServices;
using Windows.UI.Xaml;
using MORR.Modules.Clipboard.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using Morr.Core.UI.Controls.NotifyIcon.Utility;

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
        private static readonly ClipboardMessageSink clipboardMessageSink = new ClipboardMessageSink();
        public ClipboardCopyEventProducer() : base(new BoundedMultiConsumerChannelStrategy()) { }

        public void GetClipboardCopyEvents()
        {
            if (clipboardMessageSink != null)
            {
                clipboardMessageSink.ClipboardUpdated += OnClipboardUpdate();
            }
        }

        public void StopClipboardCopyEvents()
        {
            if (clipboardMessageSink != null)
            {
                clipboardMessageSink.Dispose();
            }
        }

        private void OnClipboardUpdate(uint messageId, IntPtr wParam, IntPtr lParam)
        {
            if (messageId != NativeMethods.WM_CLIPBOARDUPDATE)
            {
                return;
            }

            var @event = new ClipboardCopyEvent() { Text = Clipboard.GetText() };
            Enqueue(@event);
        }

        #region Native methods

        internal static class NativeMethods
        {

            public const int WM_CLIPBOARDUPDATE = 0x031D;
            public static IntPtr HWND_MESSAGE = new IntPtr(-3);

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
            public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

            #endregion
        }

        #endregion

        #region Window

        internal class ClipboardMessageSink
        {
            public delegate void ClipboardEventHandler(uint uMsg, IntPtr wParam, IntPtr lParam);

            private NativeMethods.WindowProcedureHandler internalWindowMessageHandler;

            public ClipboardMessageSink()
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
                    ClipboardUpdated?.Invoke(messageId, wParam, lParam);
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

            ~ClipboardMessageSink()
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
                internalWindowMessageHandler = null;
            }

            #endregion
        }

#endregion
    }
}
