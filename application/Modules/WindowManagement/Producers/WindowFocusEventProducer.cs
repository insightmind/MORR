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

        public void StartCapture()
        {
            GlobalHook.AddListener(WindowHookCallback, NativeMethods.MessageType.WM_ACTIVATE);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(WindowHookCallback, NativeMethods.MessageType.WM_ACTIVATE);
        }

        private void WindowHookCallback(GlobalHook.HookMessage msg)
        {
            if ((int)msg.wParam == WA_ACTIVE)
            {
                IntPtr hwnd = NativeMethods.GetForegroundWindow();
                string processName = Utility.GetProcessNameFromHwnd(hwnd);
                string windowTitle = Utility.GetWindowTitleFromHwnd(hwnd);
                WindowFocusEvent @event = new WindowFocusEvent(){IssuingModule = WindowManagementModule.Identifier, ProcessName = processName, Title = windowTitle};
                Enqueue(@event);
            }
        }
    }
}