using System;
using System.Runtime.InteropServices;

namespace MORR.Modules.Clipboard.Native
{
    public static class NativeClipboard
    {
        #region Clipboard text helper

        /// <summary>
        ///     Gets the text from the clipboard
        /// </summary>
        /// <param name="hwnd">Pointer to the window that currently has clipboard</param>
        /// <returns>String representing text from the clipboard</returns>
        public static string GetClipboardText()
        {
            OpenClipboard(GetOpenClipboardWindow());

            //Gets pointer to clipboard data in the selected format
            var clipboardDataPointer = GetClipboardData((uint)ClipboardFormat.CF_UNICODETEXT);

            var clipboardLock = GlobalLock(clipboardDataPointer);

            var text = Marshal.PtrToStringAuto(clipboardLock);

            GlobalUnlock(clipboardLock);

            CloseClipboard();

            if (text == null)
            {
                throw new Exception("Failed to get clipboard text.");
            }

            return text;
        }

        #endregion

        #region DllImports

        [DllImport("user32.dll")]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        public static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        public static extern IntPtr GetOpenClipboardWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);

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

        #region Subtypes

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

        #region Enums

        public enum ClipboardFormat
        {
            CF_UNICODETEXT = 13,
        }

        #endregion
    }
}
