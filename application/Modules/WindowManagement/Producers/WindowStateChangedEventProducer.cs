using System.Drawing;
using System.Windows;
using MORR.Modules.WindowManagement.Events;
using MORR.Modules.WindowManagement.Native;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowStateChangedEvent
    /// </summary>
    public class WindowStateChangedEventProducer : DefaultEventQueue<WindowStateChangedEvent>
    {
        private static readonly INativeWindowManagement nativeWindowManagement = new NativeWindowManagement();

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

        private readonly GlobalHook.MessageType[] listenedMessageTypes =
            {
                GlobalHook.MessageType.WM_SIZE,
                GlobalHook.MessageType.WM_ENTERSIZEMOVE,
                GlobalHook.MessageType.WM_EXITSIZEMOVE
            };

        public void StartCapture()
        {
            GlobalHook.AddListener(WindowHookCallback, listenedMessageTypes);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(WindowHookCallback, listenedMessageTypes);
            Close();
        }

        private void WindowHookCallback(GlobalHook.HookMessage msg)
        {
            // for detection of WindowState.Maximized and WindowState.Minimized
            if (msg.Type == (uint) GlobalHook.MessageType.WM_SIZE &&
                ((uint) msg.wParam == SIZE_MINIMIZED || (uint) msg.wParam == SIZE_MAXIMIZED))
            {
                WindowStateChangedEvent @event;
                if (msg.Data[0] == 3)
                {
                    @event = new WindowStateChangedEvent
                    {
                        IssuingModule = WindowManagementModule.Identifier,
                        ProcessName = "sampleProcessName",
                        Title = "sampleStateChangedTitle",
                        WindowState = (WindowState)msg.wParam
                    };
                    Enqueue(@event);
                    return;
                }

                @event = new WindowStateChangedEvent
                {
                    IssuingModule = WindowManagementModule.Identifier,
                    ProcessName = nativeWindowManagement.GetProcessNameFromHwnd(msg.Hwnd),
                    Title = nativeWindowManagement.GetProcessNameFromHwnd(msg.Hwnd),
                    // SIZE_MINIMIZED matches to the WindowState.Minimized in number
                    // SIZE_MAXIMIZED matches to the WindowState.Maximized in number
                    WindowState = (WindowState) msg.wParam
                };
                Enqueue(@event);
            }

            // for detection of WindowState.Normal (The window is restored.)
            if (msg.Type == (uint) GlobalHook.MessageType.WM_ENTERSIZEMOVE)
            {
                windowUnderChangeHwnd = (int) msg.Hwnd;
                windowRecBeforeChange = new Rectangle();
                nativeWindowManagement.GetWindowRect(windowUnderChangeHwnd, ref windowRecBeforeChange);
            }

            if (msg.Type == (uint) GlobalHook.MessageType.WM_EXITSIZEMOVE)
            {
                if (msg.Data[0] == 3)
                {
                    var @event = new WindowStateChangedEvent
                    {
                        IssuingModule = WindowManagementModule.Identifier,
                        Title = "sampleStateChangedTitle",
                        ProcessName = "sampleProcessName",
                        WindowState = SIZE_RESTORED
                    };
                    Enqueue(@event);
                    return;
                }

                windowRecAfterChange = new Rectangle();
                nativeWindowManagement.GetWindowRect(windowUnderChangeHwnd, ref windowRecAfterChange);
                if (!nativeWindowManagement.IsRectSizeEqual(windowRecBeforeChange, windowRecAfterChange))
                {
                    var @event = new WindowStateChangedEvent
                    {
                        IssuingModule = WindowManagementModule.Identifier,
                        Title = nativeWindowManagement.GetWindowTitleFromHwnd(msg.Hwnd),
                        ProcessName = nativeWindowManagement.GetProcessNameFromHwnd(msg.Hwnd),
                        WindowState = SIZE_RESTORED
                    };
                    Enqueue(@event);
                }
            }
        }
    }
}