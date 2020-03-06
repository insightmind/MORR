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

            if(wParam == GlobalHook.MessageType.WM_KEYDOWN || wParam == GlobalHook.MessageType.WM_SYSKEYDOWN)
            {
                var virtualKeyCode = lParam.VKCode;
                var pressedKey = KeyInterop.KeyFromVirtualKey((int)virtualKeyCode);
                var modifierKeys = GetModifierKeys();

                byte[] keyState = new byte[256];
                nativeKeyboard.GetKeyboardState(keyState);
                System.Text.StringBuilder sbString = new System.Text.StringBuilder(256);

                nativeKeyboard.ToUnicodeEx((uint)(lParam.VKCode),
                    0, keyState, sbString, sbString.Capacity, 0, IntPtr.Zero);

                string keyString = sbString.ToString();
                char key = '\0';
                if(!String.IsNullOrEmpty(keyString)) key = sbString.ToString()[0];

                var keyboardEvent = new KeyboardInteractEvent
                {
                    MappedCharacter = key,
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