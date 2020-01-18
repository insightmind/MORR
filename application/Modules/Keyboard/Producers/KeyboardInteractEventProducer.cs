using System;
using System.Composition;
using System.Runtime.InteropServices;
using System.Windows.Input;
using MORR.Modules.Keyboard.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.Keyboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for KeyboardInteractEvent
    /// </summary>
    [Export(typeof(KeyboardInteractEventProducer))]
    [Export(typeof(EventQueue<KeyboardInteractEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class KeyboardInteractEventProducer : EventQueue<KeyboardInteractEvent>
    {
        private IntPtr hook = IntPtr.Zero;

        #region constructor

        /// <summary>
        ///     Construct a KeyboardInteractEventProducer with a certain strategy.
        /// </summary>
        /// <param name="storageStrategy"></param>
        public KeyboardInteractEventProducer() : base(new KeepAllStorageStrategy()) { }

        #endregion


        private static class NativeMethods
        {
            #region Constant, Structure and Delegate Definitions

            public struct KeyboardHookStruct
            {
                public int vkCode;
                public int scanCode;
                public int flags;
                public int time;
                public int dwExtraInfo;
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

            public delegate int LowLevelKeyboardProc(int code, int wParam, ref KeyboardHookStruct lParam);

            #endregion

            #region DLL imports

            /// <summary>
            ///     Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
            /// </summary>
            /// <param name="idHook">The id of the event you want to hook</param>
            /// <param name="lpfn">The low level keyboard procedure callback.</param>
            /// <param name="hMod">The handle you want to attach the event to, can be null</param>
            /// <param name="dwThreadId">The thread you want to attach the event to, can be null</param>
            /// <returns>a handle to the desired hook</returns>
            [DllImport("user32.dll")]
            public static extern IntPtr SetWindowsHookEx(int idHook,
                                                         LowLevelKeyboardProc lpfn,
                                                         IntPtr hMod,
                                                         uint dwThreadId);

            /// <summary>
            ///     Unhooks the windows hook.
            /// </summary>
            /// <param name="hhk">The hook handle that was returned from SetWindowsHookEx</param>
            /// <returns>True if successful, false otherwise</returns>
            [DllImport("user32.dll")]
            public static extern bool UnhookWindowsHookEx(IntPtr hhk);

            /// <summary>
            ///     Calls the next hook.
            /// </summary>
            /// <param name="idHook">The hook id</param>
            /// <param name="nCode">The hook code</param>
            /// <param name="wParam">The wparam.</param>
            /// <param name="lParam">The lparam.</param>
            /// <returns></returns>
            [DllImport("user32.dll")]
            public static extern int
                CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookStruct lParam);

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true,
                       CallingConvention = CallingConvention.Winapi)]
            public static extern short GetKeyState(int keyCode);

            [DllImport("kernel32.dll")]
            public static extern uint GetCurrentThreadId();

            #endregion
        }


        #region public methods

        /// <summary>
        ///     Set the hook for the keyboard.
        /// </summary>
        public void HookKeyboard()
        {
            hook = NativeMethods.SetWindowsHookEx((int) NativeMethods.HookType.WH_KEYBOARD_LL, HookProc, IntPtr.Zero,
                                                  NativeMethods.GetCurrentThreadId());
        }

        /// <summary>
        ///     Release the hook for the keyboard.
        /// </summary>
        public void UnhookKeyboard()
        {
            NativeMethods.UnhookWindowsHookEx(hook);
        }

        #endregion

        #region private methods

        /// <summary>
        ///     The callback for the keyboard hook
        /// </summary>
        /// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The keyhook event information</param>
        /// <returns></returns>
        private int HookProc(int nCode, int wParam, ref NativeMethods.KeyboardHookStruct lParam)
        {
            if (nCode >= 0 && wParam == (int) NativeMethods.MessageType.WM_KEYDOWN)
            {
                // get the key enum element from the lParam 
                var vkCode = lParam.vkCode;

                //get both the pressed key and the modifierKeys
                var modifierKeys = GetModifierKeys();
                var pressedkey = KeyInterop.KeyFromVirtualKey(vkCode);

                //create the corresponding new Event
                var @event = new KeyboardInteractEvent { ModifierKeys = modifierKeys, PressedKey = pressedkey };

                //enqueue the new event.
                Enqueue(@event);
            }

            return NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, ref lParam);
        }

        /// <summary>
        ///     Checks whether Alt, Shift, Control or CapsLock
        ///     is pressed at the same time as the hooked key
        ///     and get all these modifier keys as ModifierKeys.
        /// </summary>
        /// <returns>the modifier keys</returns>
        private ModifierKeys GetModifierKeys()
        {
            var modifierKeys = ModifierKeys.None;

            //Alt
            if (isPressed(NativeMethods.VirtualKeyCode.VK_MENU))
            {
                modifierKeys += (int) ModifierKeys.Alt;
            }

            //Control
            if (isPressed(NativeMethods.VirtualKeyCode.VK_CONTROL))
            {
                modifierKeys += (int) ModifierKeys.Control;
            }

            //Shift
            if (isPressed(NativeMethods.VirtualKeyCode.VK_SHIFT))
            {
                modifierKeys += (int) ModifierKeys.Shift;
            }

            //Windows
            if (isPressed(NativeMethods.VirtualKeyCode.VK_LWIN) || isPressed(NativeMethods.VirtualKeyCode.VK_RWIN))
            {
                modifierKeys += (int) ModifierKeys.Windows;
            }

            return modifierKeys;
        }

        /// <summary>
        ///     Return true if a key is being pressed.
        /// </summary>
        /// <param name="vkCode"></param>
        /// <returns>true if a key is being pressed</returns>
        private bool isPressed(NativeMethods.VirtualKeyCode vkCode)
        {
            return GetLowOrderBit((int) vkCode) != 0;
        }

        /// <summary>
        ///     Get the low-order bit of a Int32 number
        /// </summary>
        /// <param name="number"></param>
        /// <returns>the low-order bit of a Int32 number</returns>
        private int GetLowOrderBit(int number)
        {
            return number & 0x0001;
        }

        #endregion
    }
}