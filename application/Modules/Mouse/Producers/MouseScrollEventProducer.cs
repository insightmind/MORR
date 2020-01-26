using System;
using System.ComponentModel.Composition;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseScrollEvent
    /// </summary>
    [Export(typeof(MouseScrollEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<MouseScrollEvent>))]
    [Export(typeof(IReadWriteEventQueue<MouseScrollEvent>))]
    [Export(typeof(IReadOnlyEventQueue<Event>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MouseScrollEventProducer : DefaultEventQueue<MouseScrollEvent>
    {
        private NativeMethods.LowLevelMouseProc? callback;

        /// <summary>
        ///     The low level mouse MouseHookHandle
        /// </summary>
        private IntPtr MouseHookHandle;

        public void StartCapture()
        {
            callback = MouseHookCallback; // Store callback to prevent GC
            if (!NativeMethods.TrySetMouseHook(callback, out MouseHookHandle))
            {
                throw new Exception("Failed hook mouse.");
            }
        }

        public void StopCapture()
        {
            if (!NativeMethods.UnhookWindowsHookEx(MouseHookHandle))
            {
                throw new Exception("Failed to unhook mouse.");
            }
        }


        /// <summary>
        ///     The callback for the Mouse hook
        ///     Create MouseScrollEvent when user scrolls the wheel
        /// </summary>
        /// <param name="nCode">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The mouse event information</param>
        /// <returns></returns>
        private int MouseHookCallback(int nCode,
                                      NativeMethods.MessageType wParam,
                                      NativeMethods.MSLLHOOKSTRUCT lParam)
        {
            if (nCode < 0)
            {
                // Required as per documentation
                // see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644985(v=vs.85)#return-value
                return NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            }

            if (wParam == NativeMethods.MessageType.WM_MOUSEWHEEL)
            {
                /// get the hookStruct from the lParam and retrieve the mousedata from it 
                var mousedata = lParam.mouseData;

                //If the message is WM_MOUSEWHEEL, the high-order word of mouseData member is the wheel delta. 
                //One wheel click is defined as WHEEL_DELTA, which is 120. 
                //(value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
                var scrollAmount = (short) ((mousedata >> 16) & 0xffff);
                var mousePosition = lParam.pt;

                //TODO get the Intptr of the window
            IntPtr hwnd = IntPtr.Zero;

                //Create corresponding event MouseScrollEvent and enqueue it
                var @event = new MouseScrollEvent() { ScrollAmount = scrollAmount, MousePosition = mousePosition,HWnd = hwnd};
                Enqueue(@event);
            }

            return NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }
    }
}