using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

        #region SetHook helper

        /// <summary>
        ///     Sets a low-level keyboard hook.
        /// </summary>
        /// <param name="callback">The callback of the hook.</param>
        /// <param name="handle">The handle of the hook. Valid if the method returns <see langword="true" /></param>
        /// <returns><see langword="true" /> if the hook could successfully be set, <see langword="false" /> otherwise.</returns>
        public static bool TrySetKeyboardHook(LowLevelKeyboardProc callback, [NotNullWhen(true)] out IntPtr handle)
        {
            using var currentProcess = Process.GetCurrentProcess();
            using var currentModule = currentProcess.MainModule;

            if (currentModule == null)
            {
                handle = IntPtr.Zero;
                return false;
            }

            var moduleHandle = GetModuleHandle(currentModule.ModuleName);
            handle = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, callback, moduleHandle, 0);
            return true;
        }

        public static bool TrySetClipboardCutHook(LowLevelClipboardProc callback, [NotNullWhen(true)] out IntPtr handle)
        {
            using var currentProcess = Process.GetCurrentProcess();
            using var currentModule = currentProcess.MainModule;

            if (currentModule == null)
            {
                handle = IntPtr.Zero;
                return false;
            }

            var moduleHandle = GetModuleHandle(currentModule.ModuleName);
            handle = SetWindowsHookEx(HookType.WH_CALLWNDPROC, callback, moduleHandle, 0);
            return true;
        }

        public static bool TrySetClipboardPasteHook(LowLevelClipboardProc callback, [NotNullWhen(true)] out IntPtr handle)
        {
            using var currentProcess = Process.GetCurrentProcess();
            using var currentModule = currentProcess.MainModule;

            if (currentModule == null)
            {
                handle = IntPtr.Zero;
                return false;
            }

            var moduleHandle = GetModuleHandle(currentModule.ModuleName);
            handle = SetWindowsHookEx(HookType.WH_CALLWNDPROC, callback, moduleHandle, 0);
            return true;
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

        #region Clipboard text helper
        /// <summary>
        ///     Gets the text from the clipboard
        /// </summary>
        /// <param name="hwnd">Pointer to the window that currently has clipboard</param>
        /// <returns>String representing text from the clipboard</returns>
        public static string getClipboardText()
        { 
            uint CF_TEXT = 1;

            OpenClipboard(GetOpenClipboardWindow());

            //Gets pointer to clipboard data in the selected format
            IntPtr ClipboardDataPointer = GetClipboardData(CF_TEXT);

            //Locks the handle to get the actual text pointer
            UIntPtr Length = GlobalSize(ClipboardDataPointer);
            IntPtr gLock = GlobalLock(ClipboardDataPointer);

            string text;

            text = Marshal.PtrToStringAuto(gLock);

            GlobalUnlock(gLock);

            CloseClipboard();

            return text;
        }

        #endregion

        #region Methods

        [DllImport("user32.dll")]
        public static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        public static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage([In] ref MSG lpMsg);

        public delegate int LowLevelKeyboardProc(int nCode, MessageType wParam, [In] KBDLLHOOKSTRUCT lParam);

        public delegate int LowLevelClipboardProc(int code, MessageType wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(VirtualKeyCode nVirtualKeyCode);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType hookType,
                                                     LowLevelKeyboardProc lpFn,
                                                     IntPtr hMod,
                                                     uint dwThreadId);
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType hookType,
                                                     LowLevelClipboardProc lpFn,
                                                     IntPtr hMod,
                                                     uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, MessageType wParam, [In] KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, MessageType wParam, int lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);


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
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern UIntPtr GlobalSize(IntPtr hMem);

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
        }

        public struct KBDLLHOOKSTRUCT
        {
            public int VKCode;
            public int ScanCode;
            public int Flags;
            public int Time;
            public int DWExtraInfo;
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
            WH_CALLWNDPROC = 4
        }

        public enum MessageType
        {
            WM_KEYDOWN = 0x100,
            WM_CUT = 0x0300,
            WM_PASTE = 0x0302,
            WM_CLIPBOARDUPDATE = 0x031D
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