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

        private readonly GlobalHook.MessageType[] listenedMessagesTypes =
        {
            GlobalHook.MessageType.WM_KEYDOWN,
            GlobalHook.MessageType.WM_SYSKEYDOWN
        };

        public void StartCapture(INativeKeyboard nativeK)
        {
            nativeKeyboard = nativeK;
            GlobalHook.AddListener(KeyboardHookCallback, listenedMessagesTypes);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(KeyboardHookCallback, listenedMessagesTypes);
            base.Close();
        }

        private void KeyboardHookCallback(GlobalHook.HookMessage hookMessage) {
            var virtualKeyCode = hookMessage.wParam;
            var pressedKey = KeyInterop.KeyFromVirtualKey((int)virtualKeyCode);
            var modifierKeys = GetModifierKeys();

            byte[] keyState = new byte[256];
            nativeKeyboard.GetKeyboardState(keyState);
            System.Text.StringBuilder sbString = new System.Text.StringBuilder(256);

            nativeKeyboard.ToUnicodeEx((uint)(virtualKeyCode),
                0, keyState, sbString, sbString.Capacity, 0, IntPtr.Zero);

            string keyString = sbString.ToString();
            char key = '\0';
            if (!String.IsNullOrEmpty(keyString)) key = sbString.ToString()[0];

            var keyboardEvent = new KeyboardInteractEvent
            {
                MappedCharacter_Unicode = key,
                PressedKey_System_Windows_Input_Key = pressedKey,
                ModifierKeys_System_Windows_Input_ModifierKeys = modifierKeys,
                IssuingModule = KeyboardModule.Identifier,
                PressedKeyName = pressedKey.ToString(),
                ModifierKeysName = modifierKeys.ToString()
            };
            Enqueue(keyboardEvent);
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