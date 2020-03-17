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
            var pressedKey = nativeKeyboard.KeyFromVirtualKey((int)virtualKeyCode);
            var modifierKeys = GetModifierKeys();
            char key = nativeKeyboard.ToUnicode((uint)virtualKeyCode);

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