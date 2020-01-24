using System;
using System.Composition;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseScrollEvent
    /// </summary>
    [Export(typeof(MouseScrollEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<MouseScrollEvent>))]
    public class MouseScrollEventProducer : DefaultEventQueue<MouseScrollEvent>
    {
        #region private fields

        /// <summary>
        ///     The low level mouse hook
        /// </summary>
        private IntPtr hook = IntPtr.Zero;

        #endregion

        #region constructor

        /// <summary>
        ///     Initialize a MouseScrollEventProducer
        /// </summary>
        public MouseScrollEventProducer() : base(new KeepAllStorageStrategy()) { }

        #endregion

        #region private methods

        /// <summary>
        ///     The callback for the Mouse hook
        ///     Create MouseScrollEvent when user scrolls the wheel
        /// </summary>
        /// <param name="nCode">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The mouse event information</param>
        /// <returns></returns>
        private int HookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (int) NativeMethods.MessageType.WM_MOUSEWHEEL)
            {
                /// get the hookStruct from the lParam and retrieve the mousedata from it 
                var hookStruct =
                    (NativeMethods.MouseHookStruct) Marshal.PtrToStructure(
                        lParam, typeof(NativeMethods.MouseHookStruct));
                var mousedata = hookStruct.mouseData;

                //If the message is WM_MOUSEWHEEL, the high-order word of mouseData member is the wheel delta. 
                //One wheel click is defined as WHEEL_DELTA, which is 120. 
                //(value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
                var scrollAmount = (short) ((mousedata >> 16) & 0xffff);
                var mousePosition = hookStruct.pt;

                //Create corresponding event MouseScrollEvent and enqueue it
                var @event = new MouseScrollEvent();
                @event.ScrollAmount = scrollAmount;
                @event.MousePosition = mousePosition;
                Enqueue(@event);
            }

            return NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        #endregion

        #region public methods

        /// <summary>
        ///     Set the hook for the mouse.
        /// </summary>
        public void HookMouse()
        {
            var currentProcess = Process.GetCurrentProcess();
            var currentModule = currentProcess.MainModule;
            var moduleName = currentModule.ModuleName;
            var moduleHandle = NativeMethods.GetModuleHandle(moduleName);
            hook = NativeMethods.SetWindowsHookEx((int) NativeMethods.HookType.WH_MOUSE_LL, HookProc, moduleHandle,
                                                  0);
        }

        /// <summary>
        ///     Release the hook for the mouse.
        /// </summary>
        public void UnhookMouse()
        {
            NativeMethods.UnhookWindowsHookEx(hook);
        }

        #endregion
    }
}