using System;
using System.Runtime.InteropServices;

namespace MORR.Modules.Clipboard.Native
{
    public interface INativeClipboard
    {
        public delegate IntPtr WindowProcedureHandler(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        string GetClipboardText();

        short RegisterClass(ref WindowClass lpWndClass);

        IntPtr CreateWindowEx(int dwExStyle,
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

        bool AddClipboardFormatListener(IntPtr hwnd);

        IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        bool RemoveClipboardFormatListener(IntPtr hwnd);

        bool DestroyWindow(IntPtr hWnd);

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
    }
}