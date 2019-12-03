using System;
using System.Runtime.InteropServices;

namespace Morr.Core.UI.Controls.NotifyIcon.Utility
{
    internal static class NativeMethods
    {
        #region Window messages

        public enum WindowMessages
        {
            WM_MOUSEMOVE = 0x200,
            WM_LBUTTONDOWN = 0x201,
            WM_LBUTTONUP = 0x202,
            WM_LBUTTONDBLCLK = 0x203,
            WM_RBUTTONDOWN = 0x204,
            WM_RBUTTONUP = 0x205,
            WM_RBUTTONDBLCLK = 0x206,
            WM_MBUTTONDOWN = 0x207,
            WM_MBUTTONUP = 0x208,
            WM_MBUTTONDBLCLK = 0x209
        }

        #endregion

        #region Structs

        [Flags]
        public enum NotifyIconFlags
        {
            NIF_MESSAGE = 0x01,
            NIF_ICON = 0x02,
            NIF_TIP = 0x04,
            NIF_STATE = 0x08,
            NIF_INFO = 0x10,
            NIF_GUID = 0x20,
            NIF_REALTIME = 0x40,
            NIF_SHOWTIP = 0x80
        }

        public enum NotifyIconInfoFlags
        {
            NIIF_NONE = 0x00,
            NIIF_INFO = 0x01,
            NIIF_WARNING = 0x02,
            NIIF_ERROR = 0x03,
            NIIF_USER = 0x04,
            NIIF_NOSOUND = 0x10,
            NIIF_LARGE_ICON = 0x20,
            NIIF_RESPECT_QUIET_TIME = 0x80,
            NIIF_ICON_MASK = 0x0F
        }

        public enum NotifyIconMessage
        {
            NIM_ADD = 0x00,
            NIM_MODIFY = 0x01,
            NIM_DELETE = 0x02,
            NIM_SETFOCUS = 0x03,
            NIM_SETVERSION = 0x04
        }

        public enum NotifyIconState
        {
            NIS_HIDDEN = 0x01,
            NIS_SHAREDICON = 0x02
        }

        public enum NotifyIconVersion
        {
            NOTIFY_ICON_VERSION = 0x3,
            NOTIFY_ICON_VERSION_4 = 0x4
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct NotifyIconData
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uID;
            public NotifyIconFlags uFlags;
            public uint uCallbackMessage;
            public IntPtr hIcon;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTip;

            public NotifyIconState dwState;
            public NotifyIconState dwStateMask;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szInfo;

            public uint uTimeoutOrVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szInfoTitle;

            public NotifyIconInfoFlags dwInfoFlags;
            public Guid guidItem;

            public IntPtr hBalloonIcon;
        }

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

        [StructLayout(LayoutKind.Sequential)]
        public struct Win32Point
        {
            public int X;
            public int Y;
        }

        #endregion

        #region Methods

        public delegate IntPtr WindowProcedureHandler(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("shell32.dll")]
        public static extern bool Shell_NotifyIcon(NotifyIconMessage dwMessage, [In] ref NotifyIconData lpData);

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

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "RegisterClassW")]
        public static extern short RegisterClass(ref WindowClass lpWndClass);

        [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageW")]
        public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

        [DllImport("user32.dll")]
        public static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Win32Point lpPoint);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion
    }
}