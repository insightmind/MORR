using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MORR.Shared.Utility
{
    /// <summary>
    ///     Contains Win32-imported methods, structs, enums and related helper methods.
    /// </summary>
    public static class NativeMethods
    {
        #region Windows message loop helper

        /// <summary>
        ///     Runs a standard Win32 message loop
        ///     <remarks>
        ///         Intended for use in contexts where a Win32 message loop is required for Windows-Hooks and no such loop
        ///         already exists (e.g. ConsoleApp).
        ///     </remarks>
        /// </summary>
        public static void DoWin32MessageLoop()
        {
            int status;
            while ((status = GetMessage(out var msg, IntPtr.Zero, 0, 0)) != 0)
            {
                // -1 indicates error - do not process such messages
                if (status != -1)
                {
                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }
            }
        }

        #endregion

        #region Keyboard state helper

        /// <summary>
        ///     Indicates whether a key identified by its virtual key code is pressed
        /// </summary>
        /// <param name="virtualKeyCode">The <see cref="VirtualKeyCode" /> of the key to check.</param>
        /// <returns><see langword="true" /> if the key is pressed, <see langword="false" /> otherwise.</returns>
        public static bool IsKeyPressed(VirtualKeyCode virtualKeyCode)
        {
            return Convert.ToBoolean(GetKeyState(virtualKeyCode) & (int) KeyMask.KEY_PRESSED);
        }

        #endregion

        #region SetHook helper

        private static bool TryGetCurrentModuleHandle(out IntPtr handle)
        {
            using var currentProcess = Process.GetCurrentProcess();
            using var currentModule = currentProcess.MainModule;
            if (currentModule == null)
            {
                handle = IntPtr.Zero;
                return false;
            }

            handle = GetModuleHandle(currentModule.ModuleName);
            return true;
        }

        /// <summary>
        ///     Sets a low-level keyboard hook.
        /// </summary>
        /// <param name="callback">The callback of the hook.</param>
        /// <param name="handle">The handle of the hook. Valid if the method returns <see langword="true" /></param>
        /// <returns><see langword="true" /> if the hook could successfully be set, <see langword="false" /> otherwise.</returns>
        public static bool TrySetKeyboardHook(LowLevelKeyboardProc callback, out IntPtr handle)
        {
            if (!TryGetCurrentModuleHandle(out var moduleHandle))
            {
                handle = IntPtr.Zero;
                return false;
            }


            handle = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, callback, moduleHandle, 0);
            return true;
        }


        /// <summary>
        ///     Sets a low-level mouse hook.
        /// </summary>
        /// <param name="callback">The callback of the hook.</param>
        /// <param name="handle">The handle of the hook. Valid if the method returns <see langword="true" /></param>
        /// <returns><see langword="true" /> if the hook could successfully be set, <see langword="false" /> otherwise.</returns>
        public static bool TrySetMouseHook(LowLevelMouseProc callback, out IntPtr handle)
        {
            if (!TryGetCurrentModuleHandle(out var moduleHandle))
            {
                handle = IntPtr.Zero;
                return false;
            }


            handle = SetWindowsHookEx(HookType.WH_MOUSE_LL, callback, moduleHandle, 0);
            return true;
        }

        #endregion

        #region Window for process helper

        /// <summary>
        ///     Attempts to get a window associated with the current process.
        /// </summary>
        /// <returns>The handle of a window associated with the current process.</returns>
        public static IntPtr GetAssociatedWindow()
        {
            var activeWindow = GetActiveWindow();
            var consoleWindow = GetConsoleWindow();

            return activeWindow != IntPtr.Zero ? activeWindow : consoleWindow;
        }

        #endregion

        #region Methods

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("user32.dll")]
        public static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        public static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage([In] ref MSG lpMsg);

        public delegate int LowLevelKeyboardProc(int nCode, MessageType wParam, [In] KBDLLHOOKSTRUCT lParam);

        public delegate int LowLevelMouseProc(int nCode, MessageType wParam, [In] MSLLHOOKSTRUCT lParam);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(VirtualKeyCode nVirtualKeyCode);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType hookType,
                                                     LowLevelKeyboardProc lpFn,
                                                     IntPtr hMod,
                                                     uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType hookType,
                                                     LowLevelMouseProc lpFn,
                                                     IntPtr hMod,
                                                     uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, MessageType wParam, [In] KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, MessageType wParam, [In] MSLLHOOKSTRUCT lParam);


        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern uint GetDoubleClickTime();
        #endregion

        #region Structs

        public struct MSG
        {
            public IntPtr HWnd;
            public uint Message;
            public IntPtr WParam;
            public IntPtr LParam;
            public uint Time;
            public POINT Pt;
        }

        /// <summary>
        ///     The POINT is of two int(32 bits) for the usage in MSLLHOOKSTRUCT.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static implicit operator Point(POINT p)
            {
                return new Point(p.X, p.Y);
            }

            public static implicit operator POINT(Point p)
            {
                return new POINT(p.X, p.Y);
            }

            public static implicit operator System.Windows.Point(POINT p)
            {
                return new System.Windows.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Windows.Point p)
            {
                return new POINT((int) p.X, (int) p.Y);
            }
        }

        public struct KBDLLHOOKSTRUCT
        {
            public int VKCode;
            public int ScanCode;
            public int Flags;
            public int Time;
            public int DWExtraInfo;
        }


        /// <summary>
        ///     The POINT in this structure is of two 32 bits Integer.
        ///     see https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-msllhookstruct
        /// </summary>
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        #endregion

        #region Enums

        public enum KeyMask
        {
            KEY_PRESSED = 0x8000,
            KEY_TOGGLED = 0x1
        }

        public enum HookType
        {
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public enum MessageType
        {
            WM_NULL = 0x00,
            WM_CREATE = 0x01,
            WM_DESTROY = 0x02,
            WM_MOVE = 0x03,
            WM_SIZE = 0x05,
            WM_ACTIVATE = 0x06,
            WM_SETFOCUS = 0x07,
            WM_KILLFOCUS = 0x08,
            WM_ENABLE = 0x0A,
            WM_SETREDRAW = 0x0B,
            WM_SETTEXT = 0x0C,
            WM_GETTEXT = 0x0D,
            WM_GETTEXTLENGTH = 0x0E,
            WM_PAINT = 0x0F,
            WM_CLOSE = 0x10,
            WM_QUERYENDSESSION = 0x11,
            WM_QUIT = 0x12,
            WM_QUERYOPEN = 0x13,
            WM_ERASEBKGND = 0x14,
            WM_SYSCOLORCHANGE = 0x15,
            WM_ENDSESSION = 0x16,
            WM_SYSTEMERROR = 0x17,
            WM_SHOWWINDOW = 0x18,
            WM_CTLCOLOR = 0x19,
            WM_WININICHANGE = 0x1A,
            WM_SETTINGCHANGE = 0x1A,
            WM_DEVMODECHANGE = 0x1B,
            WM_ACTIVATEAPP = 0x1C,
            WM_FONTCHANGE = 0x1D,
            WM_TIMECHANGE = 0x1E,
            WM_CANCELMODE = 0x1F,
            WM_SETCURSOR = 0x20,
            WM_MOUSEACTIVATE = 0x21,
            WM_CHILDACTIVATE = 0x22,
            WM_QUEUESYNC = 0x23,
            WM_GETMINMAXINFO = 0x24,
            WM_PAINTICON = 0x26,
            WM_ICONERASEBKGND = 0x27,
            WM_NEXTDLGCTL = 0x28,
            WM_SPOOLERSTATUS = 0x2A,
            WM_DRAWITEM = 0x2B,
            WM_MEASUREITEM = 0x2C,
            WM_DELETEITEM = 0x2D,
            WM_VKEYTOITEM = 0x2E,
            WM_CHARTOITEM = 0x2F,

            WM_SETFONT = 0x30,
            WM_GETFONT = 0x31,
            WM_SETHOTKEY = 0x32,
            WM_GETHOTKEY = 0x33,
            WM_QUERYDRAGICON = 0x37,
            WM_COMPAREITEM = 0x39,
            WM_COMPACTING = 0x41,
            WM_WINDOWPOSCHANGING = 0x46,
            WM_WINDOWPOSCHANGED = 0x47,
            WM_POWER = 0x48,
            WM_COPYDATA = 0x4A,
            WM_CANCELJOURNAL = 0x4B,
            WM_NOTIFY = 0x4E,
            WM_INPUTLANGCHANGEREQUEST = 0x50,
            WM_INPUTLANGCHANGE = 0x51,
            WM_TCARD = 0x52,
            WM_HELP = 0x53,
            WM_USERCHANGED = 0x54,
            WM_NOTIFYFORMAT = 0x55,
            WM_CONTEXTMENU = 0x7B,
            WM_STYLECHANGING = 0x7C,
            WM_STYLECHANGED = 0x7D,
            WM_DISPLAYCHANGE = 0x7E,
            WM_GETICON = 0x7F,
            WM_SETICON = 0x80,

            WM_NCCREATE = 0x81,
            WM_NCDESTROY = 0x82,
            WM_NCCALCSIZE = 0x83,
            WM_NCHITTEST = 0x84,
            WM_NCPAINT = 0x85,
            WM_NCACTIVATE = 0x86,
            WM_GETDLGCODE = 0x87,
            WM_NCMOUSEMOVE = 0xA0,
            WM_NCLBUTTONDOWN = 0xA1,
            WM_NCLBUTTONUP = 0xA2,
            WM_NCLBUTTONDBLCLK = 0xA3,
            WM_NCRBUTTONDOWN = 0xA4,
            WM_NCRBUTTONUP = 0xA5,
            WM_NCRBUTTONDBLCLK = 0xA6,
            WM_NCMBUTTONDOWN = 0xA7,
            WM_NCMBUTTONUP = 0xA8,
            WM_NCMBUTTONDBLCLK = 0xA9,

            WM_KEYFIRST = 0x100,
            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101,
            WM_CHAR = 0x102,
            WM_DEADCHAR = 0x103,
            WM_SYSKEYDOWN = 0x104,
            WM_SYSKEYUP = 0x105,
            WM_SYSCHAR = 0x106,
            WM_SYSDEADCHAR = 0x107,
            WM_KEYLAST = 0x108,

            WM_IME_STARTCOMPOSITION = 0x10D,
            WM_IME_ENDCOMPOSITION = 0x10E,
            WM_IME_COMPOSITION = 0x10F,
            WM_IME_KEYLAST = 0x10F,

            WM_INITDIALOG = 0x110,
            WM_COMMAND = 0x111,
            WM_SYSCOMMAND = 0x112,
            WM_TIMER = 0x113,
            WM_HSCROLL = 0x114,
            WM_VSCROLL = 0x115,
            WM_INITMENU = 0x116,
            WM_INITMENUPOPUP = 0x117,
            WM_MENUSELECT = 0x11F,
            WM_MENUCHAR = 0x120,
            WM_ENTERIDLE = 0x121,

            WM_CTLCOLORMSGBOX = 0x132,
            WM_CTLCOLOREDIT = 0x133,
            WM_CTLCOLORLISTBOX = 0x134,
            WM_CTLCOLORBTN = 0x135,
            WM_CTLCOLORDLG = 0x136,
            WM_CTLCOLORSCROLLBAR = 0x137,
            WM_CTLCOLORSTATIC = 0x138,

            WM_MOUSEMOVE = 0x200,
            WM_MOUSEFIRST = 0x200,
            WM_LBUTTONDOWN = 0x201,
            WM_LBUTTONUP = 0x202,
            WM_LBUTTONDBLCLK = 0x203,
            WM_RBUTTONDOWN = 0x204,
            WM_RBUTTONUP = 0x205,
            WM_RBUTTONDBLCLK = 0x206,
            WM_MBUTTONDOWN = 0x207,
            WM_MBUTTONUP = 0x208,
            WM_MBUTTONDBLCLK = 0x209,
            WM_MOUSEWHEEL = 0x20A,
            WM_MOUSEHWHEEL = 0x20E,

            WM_PARENTNOTIFY = 0x210,
            WM_ENTERMENULOOP = 0x211,
            WM_EXITMENULOOP = 0x212,
            WM_NEXTMENU = 0x213,
            WM_SIZING = 0x214,
            WM_CAPTURECHANGED = 0x215,
            WM_MOVING = 0x216,
            WM_POWERBROADCAST = 0x218,
            WM_DEVICECHANGE = 0x219,

            WM_MDICREATE = 0x220,
            WM_MDIDESTROY = 0x221,
            WM_MDIACTIVATE = 0x222,
            WM_MDIRESTORE = 0x223,
            WM_MDINEXT = 0x224,
            WM_MDIMAXIMIZE = 0x225,
            WM_MDITILE = 0x226,
            WM_MDICASCADE = 0x227,
            WM_MDIICONARRANGE = 0x228,
            WM_MDIGETACTIVE = 0x229,
            WM_MDISETMENU = 0x230,
            WM_ENTERSIZEMOVE = 0x231,
            WM_EXITSIZEMOVE = 0x232,
            WM_DROPFILES = 0x233,
            WM_MDIREFRESHMENU = 0x234,

            WM_IME_SETCONTEXT = 0x281,
            WM_IME_NOTIFY = 0x282,
            WM_IME_CONTROL = 0x283,
            WM_IME_COMPOSITIONFULL = 0x284,
            WM_IME_SELECT = 0x285,
            WM_IME_CHAR = 0x286,
            WM_IME_KEYDOWN = 0x290,
            WM_IME_KEYUP = 0x291,

            WM_MOUSEHOVER = 0x2A1,
            WM_NCMOUSELEAVE = 0x2A2,
            WM_MOUSELEAVE = 0x2A3,

            WM_CUT = 0x300,
            WM_COPY = 0x301,
            WM_PASTE = 0x302,
            WM_CLEAR = 0x303,
            WM_UNDO = 0x304,

            WM_RENDERFORMAT = 0x305,
            WM_RENDERALLFORMATS = 0x306,
            WM_DESTROYCLIPBOARD = 0x307,
            WM_DRAWCLIPBOARD = 0x308,
            WM_PAINTCLIPBOARD = 0x309,
            WM_VSCROLLCLIPBOARD = 0x30A,
            WM_SIZECLIPBOARD = 0x30B,
            WM_ASKCBFORMATNAME = 0x30C,
            WM_CHANGECBCHAIN = 0x30D,
            WM_HSCROLLCLIPBOARD = 0x30E,
            WM_QUERYNEWPALETTE = 0x30F,
            WM_PALETTEISCHANGING = 0x310,
            WM_PALETTECHANGED = 0x311,

            WM_HOTKEY = 0x312,
            WM_PRINT = 0x317,
            WM_PRINTCLIENT = 0x318,

            WM_HANDHELDFIRST = 0x358,
            WM_HANDHELDLAST = 0x35F,
            WM_PENWINFIRST = 0x380,
            WM_PENWINLAST = 0x38F,
            WM_COALESCE_FIRST = 0x390,
            WM_COALESCE_LAST = 0x39F,
            WM_DDE_FIRST = 0x3E0,
            WM_DDE_INITIATE = 0x3E0,
            WM_DDE_TERMINATE = 0x3E1,
            WM_DDE_ADVISE = 0x3E2,
            WM_DDE_UNADVISE = 0x3E3,
            WM_DDE_ACK = 0x3E4,
            WM_DDE_DATA = 0x3E5,
            WM_DDE_REQUEST = 0x3E6,
            WM_DDE_POKE = 0x3E7,
            WM_DDE_EXECUTE = 0x3E8,
            WM_DDE_LAST = 0x3E8,

            WM_USER = 0x400,
            WM_APP = 0x8000
    }

        public enum VirtualKeyCode
        {
            VK_SHIFT = 0x10,
            VK_CONTROL = 0x11,
            VK_MENU = 0x12,
            VK_LWIN = 0x5B,
            VK_RWIN = 0x5C
        }

        #endregion
    }
}