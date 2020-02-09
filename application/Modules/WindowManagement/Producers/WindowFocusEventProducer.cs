using System;
using MORR.Modules.WindowManagement.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowFocusEvent
    /// </summary>
    public class WindowFocusEventProducer : DefaultEventQueue<WindowFocusEvent>
    {
        private const int WA_ACTIVE = 1;

        private IntPtr lastHwnd = IntPtr.Zero;

        public override void Open()
        {
            base.Open();
            GlobalHook.AddListener(WindowHookCallback, NativeMethods.MessageType.WM_ACTIVATE);
            GlobalHook.IsActive = true;
        }

        public override void Close()
        {
            GlobalHook.RemoveListener(WindowHookCallback, NativeMethods.MessageType.WM_ACTIVATE);
            base.Close();
        }

        /// <summary>
        ///     Everytime a WM_ACTIVATE is received, check if this message contains the information
        ///     of a window being activated (by (int)msg.wParam == WA_ACTIVE) and if the Message
        ///     contains new information (by lastHwnd != hwnd). If the both requirements are met,
        ///     record the information of the activated window in a WindowFocusEvent.
        /// </summary>
        /// <param name="msg">the hook message</param>
        private void WindowHookCallback(GlobalHook.HookMessage msg)
        {
            if ((int) msg.wParam == WA_ACTIVE)
            {
                var hwnd = NativeMethods.GetForegroundWindow();
                if (lastHwnd != hwnd)
                {
                    var processName = Utility.GetProcessNameFromHwnd(hwnd);
                    var windowTitle = Utility.GetWindowTitleFromHwnd(hwnd);
                    var @event = new WindowFocusEvent
                    {
                        IssuingModule = WindowManagementModule.Identifier, ProcessName = processName,
                        Title = windowTitle
                    };
                    Enqueue(@event);
                }

                lastHwnd = hwnd;
            }
        }
    }
}