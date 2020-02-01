﻿using System;
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
        #region Properties

        /// <summary>
        /// Defines whether a current message loop should be cancelled.
        /// We use a bool type for this as it is atomic and should not be problematic concerning threading.
        /// </summary>
        private static volatile bool shouldCancel = true;

        #endregion

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
            // We set to false as we expect a call to this method to be purposeful.
            shouldCancel = false;
            while (!shouldCancel)
            {
                if (PeekMessage(out var msg, IntPtr.Zero, 0, 0, 0))
                {
                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }
            }
        }

        public static void StopMessageLoop()
        {
            shouldCancel = true;
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

        [DllImport("user32.dll")]
        public static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage([In] ref MSG lpMsg);

        public delegate int LowLevelKeyboardProc(int nCode, MessageType wParam, [In] KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(VirtualKeyCode nVirtualKeyCode);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType hookType,
                                                     LowLevelKeyboardProc lpFn,
                                                     IntPtr hMod,
                                                     uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, MessageType wParam, [In] KBDLLHOOKSTRUCT lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

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
            WH_KEYBOARD_LL = 13
        }

        public enum MessageType
        {
            WM_KEYDOWN = 0x100
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