﻿using System;
using System.Windows.Input;
using MORR.Modules.Keyboard.Events;
using MORR.Modules.Keyboard.Native;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;

namespace MORR.Modules.Keyboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for KeyboardInteractEvent
    /// </summary>
    public class KeyboardInteractEventProducer : DefaultEventQueue<KeyboardInteractEvent>
    {
        private NativeKeyboard.LowLevelKeyboardProc? callback;
        private IntPtr keyboardHookHandle;

        public void StartCapture()
        {
            callback = KeyboardHookCallback; // Store callback to prevent GC
            if (!NativeKeyboard.TrySetKeyboardHook(callback, out keyboardHookHandle))
            {
                throw new Exception("Failed hook keyboard.");
            }
        }

        public void StopCapture()
        {
            if (!NativeKeyboard.UnhookWindowsHookEx(keyboardHookHandle))
            {
                throw new Exception("Failed to unhook keyboard.");
            }

            Close();
        }

        private int KeyboardHookCallback(int nCode, GlobalHook.MessageType wParam, NativeKeyboard.KBDLLHOOKSTRUCT lParam)
        {
            if (nCode < 0)
            {
                // Required as per documentation
                // see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644985(v=vs.85)#return-value
                return NativeKeyboard.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            }

            if (wParam == GlobalHook.MessageType.WM_KEYDOWN)
            {
                var virtualKeyCode = lParam.VKCode;
                var pressedKey = KeyInterop.KeyFromVirtualKey(virtualKeyCode);

                var modifierKeys = GetModifierKeys();

                var keyboardEvent = new KeyboardInteractEvent
                {
                    PressedKey = pressedKey,
                    ModifierKeys = modifierKeys,
                    IssuingModule = KeyboardModule.Identifier
                };

                Enqueue(keyboardEvent);
            }

            return NativeKeyboard.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private static ModifierKeys GetModifierKeys()
        {
            var modifierKeys = ModifierKeys.None;

            if (NativeKeyboard.IsKeyPressed(NativeKeyboard.VirtualKeyCode.VK_MENU))
            {
                modifierKeys |= ModifierKeys.Alt;
            }

            if (NativeKeyboard.IsKeyPressed(NativeKeyboard.VirtualKeyCode.VK_CONTROL))
            {
                modifierKeys |= ModifierKeys.Control;
            }

            if (NativeKeyboard.IsKeyPressed(NativeKeyboard.VirtualKeyCode.VK_SHIFT))
            {
                modifierKeys |= ModifierKeys.Shift;
            }

            if (NativeKeyboard.IsKeyPressed(NativeKeyboard.VirtualKeyCode.VK_LWIN)
                || NativeKeyboard.IsKeyPressed(NativeKeyboard.VirtualKeyCode.VK_RWIN))
            {
                modifierKeys |= ModifierKeys.Windows;
            }

            return modifierKeys;
        }
    }
}