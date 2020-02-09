using System;
using System.Windows.Input;
using MORR.Modules.Keyboard.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.Keyboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for KeyboardInteractEvent
    /// </summary>
    public class KeyboardInteractEventProducer : DefaultEventQueue<KeyboardInteractEvent>
    {
        private NativeMethods.LowLevelKeyboardProc? callback;
        private IntPtr keyboardHookHandle;

        public void StartCapture()
        {
            callback = KeyboardHookCallback; // Store callback to prevent GC
            if (!NativeMethods.TrySetKeyboardHook(callback, out keyboardHookHandle))
            {
                throw new Exception("Failed hook keyboard.");
            }
        }

        public void StopCapture()
        {
            if (!NativeMethods.UnhookWindowsHookEx(keyboardHookHandle))
            {
                throw new Exception("Failed to unhook keyboard.");
            }

            Close();
        }

        private int KeyboardHookCallback(int nCode,
                                         NativeMethods.MessageType wParam,
                                         NativeMethods.KBDLLHOOKSTRUCT lParam)
        {
            if (nCode < 0)
            {
                // Required as per documentation
                // see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644985(v=vs.85)#return-value
                return NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            }

            if (wParam == NativeMethods.MessageType.WM_KEYDOWN)
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

            return NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private static ModifierKeys GetModifierKeys()
        {
            var modifierKeys = ModifierKeys.None;

            if (NativeMethods.IsKeyPressed(NativeMethods.VirtualKeyCode.VK_MENU))
            {
                modifierKeys |= ModifierKeys.Alt;
            }

            if (NativeMethods.IsKeyPressed(NativeMethods.VirtualKeyCode.VK_CONTROL))
            {
                modifierKeys |= ModifierKeys.Control;
            }

            if (NativeMethods.IsKeyPressed(NativeMethods.VirtualKeyCode.VK_SHIFT))
            {
                modifierKeys |= ModifierKeys.Shift;
            }

            if (NativeMethods.IsKeyPressed(NativeMethods.VirtualKeyCode.VK_LWIN)
                || NativeMethods.IsKeyPressed(NativeMethods.VirtualKeyCode.VK_RWIN))
            {
                modifierKeys |= ModifierKeys.Windows;
            }

            return modifierKeys;
        }
    }
}