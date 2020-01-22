using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Composition;
using MORR.Modules.Clipboard.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;


namespace MORR.Modules.Clipboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ClipboardPasteEvent
    /// </summary>
    [Export(typeof(ClipboardPasteEventProducer))]
    [Export(typeof(EventQueue<ClipboardPasteEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class ClipboardPasteEventProducer : EventQueue<ClipboardPasteEvent>

    {
        private IntPtr hook = IntPtr.Zero;
        /// <summary>
        /// Creates a ClipboardPasteEventProducer with bounded multiconsumer strategy
        /// </summary>
        public ClipboardPasteEventProducer() : base(new BoundedMultiConsumerChannelStrategy()) { }

        #region NativeMethods

        internal static class NativeMethods
        {
            // Hook type
            public const int WH_CALLWNDPROC = 4; 
            
            // Clipboard data format
            public const int CF_TEXT = 1;

            // Window message value
            public const int WM_PASTE = 0x0302;

            public delegate int LowLevelClipboardProc(int code, int wParam, int lParam);

            #region DLL imports

            /// <summary>
            ///     Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
            /// </summary>
            /// <param name="idHook">The id of the event you want to hook</param>
            /// <param name="lpfn">The low level clipboard procedure callback.</param>
            /// <param name="hMod">The handle you want to attach the event to, can be null</param>
            /// <param name="dwThreadId">The thread you want to attach the event to, can be null</param>
            /// <returns>a handle to the desired hook</returns>
            [DllImport("user32.dll")]
            public static extern IntPtr SetWindowsHookEx(int idHook,
                                                         LowLevelClipboardProc lpfn,
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
                CallNextHookEx(IntPtr idHook, int nCode, int wParam, int lParam);

            [DllImport("kernel32.dll")]
            public static extern IntPtr GetModuleHandle(string lpModuleName);
            /// <summary>
            /// Opens clipboard
            /// </summary>
            /// <param name="hWndNewOwner">Pointer to the window that currently has clipboard </param>
            /// <returns>true on success false otherwise</returns> 
            [DllImport("user32.dll")]
            internal static extern bool OpenClipboard(IntPtr hWndNewOwner);
            /// <summary>
            /// Closes clipboard
            /// </summary>
            /// <returns>true on success false otherwise</returns>
            [DllImport("user32.dll")]
            internal static extern bool CloseClipboard();
            /// <summary>
            /// Gets pointer to the window that currently has clipboard
            /// </summary>
            /// <returns></returns>
            [DllImport("user32.dll")]
            internal static extern IntPtr GetOpenClipboardWindow();
            /// <summary>
            /// Gets data of the clipboard
            /// </summary>
            /// <param name="uFormat">Format of clipboard data</param>
            /// <returns>Pointer to clipboard data</returns>
            [DllImport("user32.dll")]
            internal static extern IntPtr GetClipboardData(uint uFormat);

            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GlobalUnlock(IntPtr hMem);

            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern IntPtr GlobalLock(IntPtr hMem);

            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern UIntPtr GlobalSize(IntPtr hMem);

            #endregion
        }
        #endregion

        #region Public methods

        /// <summary>
        ///     Sets the hook for the clipboard paste event.
        /// </summary>
        public void HookClipboardPasteEvent()
        {
            var currentProcess = Process.GetCurrentProcess();
            var currentModule = currentProcess.MainModule;
            var moduleName = currentModule.ModuleName;
            var moduleHandle = NativeMethods.GetModuleHandle(moduleName);
            hook = NativeMethods.SetWindowsHookEx((int)NativeMethods.WH_CALLWNDPROC, HookProc, moduleHandle,
                                                  0);
        }

        /// <summary>
        ///     Releases the hook for the clipboard paste event.
        /// <summary>
        public void UnhookClipboardPasteEvent()
        {
            NativeMethods.UnhookWindowsHookEx(hook);
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     The callback for the clipboard paste event hook
        /// </summary>
        /// <param name="nCode">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The clipboard paste event hook information</param>
        /// <returns></returns>
        private int HookProc(int nCode, int wParam, int lParam)
        {
            if (nCode >= 0 && wParam == (int)NativeMethods.WM_PASTE)
            {

                var text = getTextClipboard(NativeMethods.GetOpenClipboardWindow());

                //create the corresponding new Event
                var @event = new ClipboardPasteEvent() { Text = text };

                //enqueue the new event.
                Enqueue(@event);
            }

            return NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }
        /// <summary>
        ///     Gets the text from the clipboard
        /// </summary>
        /// <param name="hwnd">Pointer to the window that currently has clipboard</param>
        /// <returns>String representing text from the clipboard</returns>
        private string getTextClipboard(IntPtr hwnd)
        {
            NativeMethods.OpenClipboard(hwnd);

            //Gets pointer to clipboard data in the selected format
            IntPtr ClipboardDataPointer = NativeMethods.GetClipboardData(NativeMethods.CF_TEXT);

            // Locks the handle to get the actual text pointer
            UIntPtr Length = NativeMethods.GlobalSize(ClipboardDataPointer);
            IntPtr gLock = NativeMethods.GlobalLock(ClipboardDataPointer);

            string text;

            text = Marshal.PtrToStringAuto(gLock);

            // Releases the lock
            NativeMethods.GlobalUnlock(gLock);

            // Releases the clipboard
            NativeMethods.CloseClipboard();

            return text;
        }
        
        #endregion
    }
}
