using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using MORR.Shared.Hook;

namespace MORR.Modules.Keyboard.Native
{
    internal class NativeKeyboard : INativeKeyboard
    {
        #region Subtypes

        public enum KeyMask
        {
            KEY_PRESSED = 0x8000
        }

        #endregion

        #region Keyboard state helper

        /// <summary>
        ///     Indicates whether a key identified by its virtual key code is pressed
        /// </summary>
        /// <param name="virtualKeyCode">The <see cref="INativeKeyboard.VirtualKeyCode" /> of the key to check.</param>
        /// <returns><see langword="true" /> if the key is pressed, <see langword="false" /> otherwise.</returns>
        public bool IsKeyPressed(INativeKeyboard.VirtualKeyCode virtualKeyCode)
        {
            return Convert.ToBoolean(GetKeyState(virtualKeyCode) & (int) KeyMask.KEY_PRESSED);
        }

        #endregion

        /// <summary>
        ///     Sets a low-level keyboard hook.
        /// </summary>
        /// <param name="callback">The callback of the hook.</param>
        /// <param name="handle">The handle of the hook. Valid if the method returns <see langword="true" /></param>
        /// <returns><see langword="true" /> if the hook could successfully be set, <see langword="false" /> otherwise.</returns>
        public bool TrySetKeyboardHook(INativeKeyboard.LowLevelKeyboardProc callback, out IntPtr handle)
        {
            if (!TryGetCurrentModuleHandle(out var moduleHandle))
            {
                handle = IntPtr.Zero;
                return false;
            }


            handle = SetWindowsHookEx(INativeKeyboard.HookType.WH_KEYBOARD_LL, callback, moduleHandle, 0);
            return true;
        }

        Key INativeKeyboard.KeyFromVirtualKey(int virtualKeyCode) 
        {
            return KeyInterop.KeyFromVirtualKey(virtualKeyCode);
        }

        int INativeKeyboard.CallNextHookEx(IntPtr hhk,
                                           int nCode,
                                           GlobalHook.MessageType wParam,
                                           [In] INativeKeyboard.KBDLLHOOKSTRUCT lParam)
        {
            return CallNextHookEx(hhk, nCode, wParam, lParam);
        }

        bool INativeKeyboard.UnhookWindowsHookEx(IntPtr hhk)
        {
            return UnhookWindowsHookEx(hhk);
        }

        IntPtr INativeKeyboard.SetWindowsHookEx(INativeKeyboard.HookType hookType,
                                                INativeKeyboard.LowLevelKeyboardProc lpFn,
                                                IntPtr hMod,
                                                uint dwThreadId)
        {
            return SetWindowsHookEx(hookType, lpFn, hMod, dwThreadId);
        }

        IntPtr INativeKeyboard.GetKeyboardLayout(uint idThread) { return GetKeyboardLayout(idThread);}

        int INativeKeyboard.ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[]
        lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff,
        int cchBuff, uint wFlags, IntPtr dwhkl)
        {
            return ToUnicodeEx(wVirtKey, wScanCode, lpKeyState, pwszBuff, cchBuff, wFlags, dwhkl
                );
        }

        bool INativeKeyboard.GetKeyboardState(byte[] lpKeyState) {
            return GetKeyboardState(lpKeyState);
        }

        private bool TryGetCurrentModuleHandle(out IntPtr handle)
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


        #region DllImports

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhk,
                                                 int nCode,
                                                 GlobalHook.MessageType wParam,
                                                 [In] INativeKeyboard.KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(INativeKeyboard.HookType hookType,
                                                      INativeKeyboard.LowLevelKeyboardProc lpFn,
                                                      IntPtr hMod,
                                                      uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(INativeKeyboard.VirtualKeyCode nVirtualKeyCode);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[]
   lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff,
   int cchBuff, uint wFlags, IntPtr dwhkl);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        #endregion
    }
}