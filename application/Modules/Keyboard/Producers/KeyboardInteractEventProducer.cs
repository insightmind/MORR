using System;
using MORR.Shared.Events.Queue;
using MORR.Modules.Keyboard.Events;
using MORR.Shared.Events;
using System.Composition;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Input;



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
        #region Constant, Structure and Delegate Definitions

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;

        private const int VK_SHIFT = 0x10;
        private const int VK_CONTROL = 0x11;
        private const int VK_MENU = 0x12;
        private const int VK_CAPITAL = 0x14;

        private IntPtr hook = IntPtr.Zero;

        private delegate int LowLevelKeyboardProc(int code, int wParam, ref KeyboardHookStruct lParam);
        private struct KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        #endregion

        #region constructor
        /// <summary>
        /// Construct a KeyboardInteractEventProducer with a certain strategy.
        /// </summary>
        /// <param name="storageStrategy"></param>
        public KeyboardInteractEventProducer() : base(new KeepAllStorageStrategy())
        {
        }
        #endregion
        

        #region public methods
        /// <summary>
        /// Set the hook for the keyboard.
        /// </summary>
        public void HookKeyboard()
        {
            Process currentProcess = Process.GetCurrentProcess();
            ProcessModule currentModule = currentProcess.MainModule;
            String moduleName = currentModule.ModuleName;
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            hook = SetWindowsHookEx(WH_KEYBOARD_LL, HookProc, moduleHandle, 0);
        }

        /// <summary>
        /// Release the hook for the keyboard.
        /// </summary>
        public void UnhookKeyboard()
        {
            UnhookWindowsHookEx(hook);
        }
        #endregion

        #region private methods
        /// <summary>
        /// The callback for the keyboard hook
        /// </summary>
        /// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The keyhook event information</param>
        /// <returns></returns>
        private int HookProc(int nCode, int wParam, ref KeyboardHookStruct lParam)
        {
            if (nCode >= 0 && wParam == WM_KEYDOWN)
            {
                // read the virtual key code from the IParam
                Keys key = (Keys)lParam.vkCode;

                // add modifiers for the keys
                key = AddModifiers(key);

                // converts the Win32 Virtual-Key into WPF Key.
                Key wpfkey = KeyInterop.KeyFromVirtualKey((int)key);

                //create corresponding new Event
                KeyboardInteractEvent @event = new KeyboardInteractEvent();
                @event.PressedKey = wpfkey;

                //enque it
                this.Enqueue(@event);
    }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, ref lParam);
        }

        /// <summary>
        /// Checks whether Alt, Shift, Control or CapsLock
        /// is pressed at the same time as the hooked key.
        /// Modifies the keyCode to include the pressed keys.
        /// </summary>
        private Keys AddModifiers(Keys key)
        {
            //CapsLock
            if ((GetKeyState(VK_CAPITAL) & 0x0001) != 0) key = key | Keys.CapsLock;

            //Shift
            if ((GetKeyState(VK_SHIFT) & 0x8000) != 0) key = key | Keys.Shift;

            //Ctrl
            if ((GetKeyState(VK_CONTROL) & 0x8000) != 0) key = key | Keys.Control;

            //Alt
            if ((GetKeyState(VK_MENU) & 0x8000) != 0) key = key | Keys.Alt;

            return key;
        }
        #endregion



        #region DLL imports
        /// <summary>
        /// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
        /// </summary>
        /// <param name="idHook">The id of the event you want to hook</param>
        /// <param name="lpfn">The low level keyboard procedure callback.</param>
        /// <param name="hMod">The handle you want to attach the event to, can be null</param>
        /// <param name="dwThreadId">The thread you want to attach the event to, can be null</param>
        /// <returns>a handle to the desired hook</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        /// <summary>
        /// Unhooks the windows hook.
        /// </summary>
        /// <param name="hhk">The hook handle that was returned from SetWindowsHookEx</param>
        /// <returns>True if successful, false otherwise</returns>
        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        /// <summary>
        /// Calls the next hook.
        /// </summary>
        /// <param name="idHook">The hook id</param>
        /// <param name="nCode">The hook code</param>
        /// <param name="wParam">The wparam.</param>
        /// <param name="lParam">The lparam.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookStruct lParam);


        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(String lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);
        #endregion
    }
}