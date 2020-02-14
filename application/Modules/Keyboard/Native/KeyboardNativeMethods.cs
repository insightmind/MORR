using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MORR.Shared.Hook;

namespace MORR.Modules.Keyboard.Native
{
    public static class KeyboardNativeMethods
    {

        #region Keyboard state helper

        /// <summary>
        ///     Indicates whether a key identified by its virtual key code is pressed
        /// </summary>
        /// <param name="virtualKeyCode">The <see cref="VirtualKeyCode" /> of the key to check.</param>
        /// <returns><see langword="true" /> if the key is pressed, <see langword="false" /> otherwise.</returns>
        public static bool IsKeyPressed(VirtualKeyCode virtualKeyCode)
        {
            return Convert.ToBoolean(GetKeyState(virtualKeyCode) & (int)KeyMask.KEY_PRESSED);
        }

        #endregion

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

        public delegate int LowLevelKeyboardProc(int nCode, GlobalHook.MessageType wParam, [In] KBDLLHOOKSTRUCT lParam);

        #region DllImports

        [DllImport("user32.dll")]
        internal static extern int CallNextHookEx(IntPtr hhk, int nCode, GlobalHook.MessageType wParam, [In] KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll")]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetWindowsHookEx(HookType hookType, LowLevelKeyboardProc lpFn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(VirtualKeyCode nVirtualKeyCode);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        #region Subtypes

        public struct KBDLLHOOKSTRUCT
        {
            public int VKCode;
            public int ScanCode;
            public int Flags;
            public int Time;
            public int DWExtraInfo;
        }

        public enum HookType
        {
            WH_KEYBOARD_LL = 13,
        }

        public enum VirtualKeyCode
        {
            VK_SHIFT = 0x10,
            VK_CONTROL = 0x11,
            VK_MENU = 0x12,
            VK_LWIN = 0x5B,
            VK_RWIN = 0x5C
        }

        public enum KeyMask
        {
            KEY_PRESSED = 0x8000,
        }

        #endregion
    }
}
