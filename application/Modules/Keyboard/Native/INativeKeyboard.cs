using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using MORR.Shared.Hook;

namespace MORR.Modules.Keyboard.Native
{
    public interface INativeKeyboard
    {
        public delegate int LowLevelKeyboardProc(int nCode, GlobalHook.MessageType wParam, [In] KBDLLHOOKSTRUCT lParam);

        public enum HookType
        {
            WH_KEYBOARD_LL = 13
        }

        public enum VirtualKeyCode
        {
            VK_SHIFT = 0x10,
            VK_CONTROL = 0x11,
            VK_MENU = 0x12,
            VK_CAPITAL = 0x14,
            VK_LWIN = 0x5B,
            VK_RWIN = 0x5C
        }

        Key KeyFromVirtualKey(Int32 virtualKeyCode);

        int CallNextHookEx(IntPtr hhk, int nCode, GlobalHook.MessageType wParam, [In] KBDLLHOOKSTRUCT lParam);

        bool UnhookWindowsHookEx(IntPtr hhk);

        IntPtr SetWindowsHookEx(HookType hookType, LowLevelKeyboardProc lpFn, IntPtr hMod, uint dwThreadId);

        bool IsKeyPressed(VirtualKeyCode virtualKeyCode);

        bool TrySetKeyboardHook(LowLevelKeyboardProc callback, out IntPtr handle);

        IntPtr GetKeyboardLayout(uint idThread);


        char ToUnicode(uint wVirtKey);

        public struct KBDLLHOOKSTRUCT
        {
            public uint VKCode;
            public uint ScanCode;
            public uint Flags;
            public uint Time;
            public uint DWExtraInfo;
        }
    }
}