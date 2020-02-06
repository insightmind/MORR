using MORR.Modules.WindowManagement.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;
using System.Windows;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowStateChangedEvent
    /// </summary>
    public class WindowStateChangedEventProducer : DefaultEventQueue<WindowStateChangedEvent>
    {
        //private const int SIZE_RESTORED = 0;
        private const int SIZE_MINIMIZED = 1;
        private const int SIZE_MAXIMIZED = 2;

        public void StartCapture()
        {
            GlobalHook.AddListener(WindowHookCallback, NativeMethods.MessageType.WM_SIZE);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(WindowHookCallback, NativeMethods.MessageType.WM_SIZE);
        }

        private void WindowHookCallback(GlobalHook.HookMessage msg)
        {
            if ((int)msg.wParam == SIZE_MINIMIZED || (int)msg.wParam == SIZE_MAXIMIZED)
            {
                WindowStateChangedEvent @event = new WindowStateChangedEvent()
                {
                    IssuingModule = WindowManagementModule.Identifier,
                    ProcessName = Utility.GetProcessNameFromHwnd(msg.Hwnd),
                    Title = Utility.GetProcessNameFromHwnd(msg.Hwnd),
                    WindowState = (WindowState)((int)msg.wParam)
                };
                Enqueue(@event);
            }
        }
    }
}