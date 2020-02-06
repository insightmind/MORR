using System.Drawing;
using System.Windows;
using MORR.Modules.WindowManagement.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowStateChangedEvent
    /// </summary>
    public class WindowStateChangedEventProducer : DefaultEventQueue<WindowStateChangedEvent>
    {
        private const int SIZE_RESTORED = 0;
        private const int SIZE_MINIMIZED = 1;
        private const int SIZE_MAXIMIZED = 2;

        /// <summary>
        ///     The rectangle that holds the size and location of the window
        ///     after the change.
        /// </summary>
        private Rectangle windowRecAfterChange;

        /// <summary>
        ///     The rectangle that holds the size and location of the window
        ///     before the change.
        /// </summary>
        private Rectangle windowRecBeforeChange;

        /// <summary>
        ///     The hwnd of the windows being changed.
        ///     Change can mean move or resize.
        /// </summary>
        private int windowUnderChangeHwnd;

        public void StartCapture()
        {
            GlobalHook.AddListener(WindowHookCallback, NativeMethods.MessageType.WM_SIZE,
                                   NativeMethods.MessageType.WM_ENTERSIZEMOVE,
                                   NativeMethods.MessageType.WM_EXITSIZEMOVE);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(WindowHookCallback, NativeMethods.MessageType.WM_SIZE,
                                      NativeMethods.MessageType.WM_ENTERSIZEMOVE,
                                      NativeMethods.MessageType.WM_EXITSIZEMOVE);
            NotifyOnEnqueueFinished();
        }

        private void WindowHookCallback(GlobalHook.HookMessage msg)
        {
            // for detection of WindowState.Maximized and WindowState.Minimized
            if (msg.Type == (uint) NativeMethods.MessageType.WM_SIZE &&
                ((uint) msg.wParam == SIZE_MINIMIZED || (uint) msg.wParam == SIZE_MAXIMIZED))
            {
                var @event = new WindowStateChangedEvent
                {
                    IssuingModule = WindowManagementModule.Identifier,
                    ProcessName = Utility.GetProcessNameFromHwnd(msg.Hwnd),
                    Title = Utility.GetProcessNameFromHwnd(msg.Hwnd),
                    // SIZE_MINIMIZED matches to the WindowState.Minimized in number
                    // SIZE_MAXIMIZED matches to the WindowState.Maximized in number
                    WindowState = (WindowState) msg.wParam
                };
                Enqueue(@event);
            }

            // for detection of WindowState.Normal (The window is restored.)
            if (msg.Type == (uint) NativeMethods.MessageType.WM_ENTERSIZEMOVE)
            {
                windowUnderChangeHwnd = (int) msg.Hwnd;
                windowRecBeforeChange = new Rectangle();
                NativeMethods.GetWindowRect(windowUnderChangeHwnd, ref windowRecBeforeChange);
            }

            if (msg.Type == (uint) NativeMethods.MessageType.WM_EXITSIZEMOVE)
            {
                windowRecAfterChange = new Rectangle();
                NativeMethods.GetWindowRect(windowUnderChangeHwnd, ref windowRecAfterChange);
                if (!Utility.IsRectSizeEqual(windowRecBeforeChange, windowRecAfterChange))
                {
                    var @event = new WindowStateChangedEvent
                    {
                        IssuingModule = WindowManagementModule.Identifier,
                        Title = Utility.GetWindowTitleFromHwnd(msg.Hwnd),
                        ProcessName = Utility.GetProcessNameFromHwnd(msg.Hwnd),
                        WindowState = SIZE_RESTORED
                    };
                    Enqueue(@event);
                }
            }
        }
    }
}