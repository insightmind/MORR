using System;
using MORR.Modules.WindowManagement.Events;
using MORR.Modules.WindowManagement.Native;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowFocusEvent
    /// </summary>
    public class WindowFocusEventProducer : DefaultEventQueue<WindowFocusEvent>
    {
        private const int WA_ACTIVE = 1;
        private static INativeWindowManagement? nativeWindowManagement;
        private IntPtr lastHwnd = IntPtr.Zero;

        public void StartCapture(INativeWindowManagement nativeWinManagement)
        {
            nativeWindowManagement = nativeWinManagement;
            GlobalHook.AddListener(WindowHookCallback, GlobalHook.MessageType.WM_ACTIVATE);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(WindowHookCallback, GlobalHook.MessageType.WM_ACTIVATE);
            Close();
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
            if ((int) msg.wParam == WA_ACTIVE && nativeWindowManagement != null)
            {
                var hwnd = nativeWindowManagement.GetForegroundWindow();
                if (lastHwnd != hwnd)
                {
                    var processName = nativeWindowManagement.GetProcessNameFromHwnd(hwnd);
                    var windowTitle = nativeWindowManagement.GetWindowTitleFromHwnd(hwnd);
                    var @event = new WindowFocusEvent
                    {
                        IssuingModule = WindowManagementModule.Identifier,
                        ProcessName = processName,
                        Title = windowTitle
                    };
                    Enqueue(@event);
                }

                lastHwnd = hwnd;
            }
        }
    }
}