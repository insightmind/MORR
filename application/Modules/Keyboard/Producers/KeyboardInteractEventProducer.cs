using System;
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
        private static INativeKeyboard nativeKeyboard;
        private INativeKeyboard.LowLevelKeyboardProc? callback;
        private IntPtr keyboardHookHandle;

        public void StartCapture(INativeKeyboard nativeKb)
        {
            nativeKeyboard = nativeKb;
            callback = KeyboardHookCallback; // Store callback to prevent GC
            if (!nativeKeyboard.TrySetKeyboardHook(callback, out keyboardHookHandle))
            {
                throw new Exception("Failed hook keyboard.");
            }
        }

        public void StopCapture()
        {
            if (!nativeKeyboard.UnhookWindowsHookEx(keyboardHookHandle))
            {
                throw new Exception("Failed to unhook keyboard.");
            }

            Close();
        }

        private int KeyboardHookCallback(int nCode,
                                         GlobalHook.MessageType wParam,
                                         INativeKeyboard.KBDLLHOOKSTRUCT lParam)
        {
            if (nCode < 0)
            {
                // Required as per documentation
                // see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644985(v=vs.85)#return-value
                return nativeKeyboard.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
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

            return nativeKeyboard.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private static ModifierKeys GetModifierKeys()
        {
            var modifierKeys = ModifierKeys.None;

            if (nativeKeyboard.IsKeyPressed(INativeKeyboard.VirtualKeyCode.VK_MENU))
            {
                modifierKeys |= ModifierKeys.Alt;
            }

            if (nativeKeyboard.IsKeyPressed(INativeKeyboard.VirtualKeyCode.VK_CONTROL))
            {
                modifierKeys |= ModifierKeys.Control;
            }

            if (nativeKeyboard.IsKeyPressed(INativeKeyboard.VirtualKeyCode.VK_SHIFT))
            {
                modifierKeys |= ModifierKeys.Shift;
            }

            if (nativeKeyboard.IsKeyPressed(INativeKeyboard.VirtualKeyCode.VK_LWIN)
                || nativeKeyboard.IsKeyPressed(INativeKeyboard.VirtualKeyCode.VK_RWIN))
            {
                modifierKeys |= ModifierKeys.Windows;
            }

            return modifierKeys;
        }
    }
}