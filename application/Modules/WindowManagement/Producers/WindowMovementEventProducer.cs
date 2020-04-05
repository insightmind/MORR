using System.Drawing;
using MORR.Modules.WindowManagement.Events;
using MORR.Modules.WindowManagement.Native;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowMovementEvent
    /// </summary>
    public class WindowMovementEventProducer : DefaultEventQueue<WindowMovementEvent>
    {
        private static INativeWindowManagement? nativeWindowManagement;

        private readonly GlobalHook.MessageType[] listenedMessageTypes =
            { GlobalHook.MessageType.WM_ENTERSIZEMOVE, GlobalHook.MessageType.WM_EXITSIZEMOVE };

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

        public void StartCapture(INativeWindowManagement nativeWinManagement)
        {
            nativeWindowManagement = nativeWinManagement;
            GlobalHook.AddListener(WindowHookCallback, listenedMessageTypes);

            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(WindowHookCallback, listenedMessageTypes);
            Close();
        }

        /// <summary>
        ///     Everytime a WM_ENTERSIZEMOVE is received,
        ///     records the information of the window before the change
        ///     and wait for the WM_EXITSIZEMOVE.
        ///     Everytime a WM_EXITSIZEMOVE is received,
        ///     records the information of the window after the change
        ///     and see if the windows has been moved
        ///     (by if (Utility.IsRectSizeEqual(windowRecBeforeChange,windowRecAfterChange)))
        ///     If so, records the relevant information into a WindowMovementEvent.
        /// </summary>
        /// <param name="msg">the hook message</param>
        private void WindowHookCallback(GlobalHook.HookMessage msg)
        {
            if (msg.Type == (uint) GlobalHook.MessageType.WM_ENTERSIZEMOVE)
            {
                windowUnderChangeHwnd = (int) msg.Hwnd;
                windowRecBeforeChange = new Rectangle();
                nativeWindowManagement?.GetWindowRect(windowUnderChangeHwnd, ref windowRecBeforeChange);
            }

            if (msg.Type == (uint) GlobalHook.MessageType.WM_EXITSIZEMOVE)
            {
                windowRecAfterChange = new Rectangle();
                nativeWindowManagement?.GetWindowRect(windowUnderChangeHwnd, ref windowRecAfterChange);
                if (nativeWindowManagement?.IsRectSizeEqual(windowRecBeforeChange, windowRecAfterChange) ?? false)
                {
                    var oldLocation = nativeWindowManagement.GetPoint(windowRecBeforeChange.X, windowRecBeforeChange.Y);
                    var newLocation = nativeWindowManagement.GetPoint(windowRecAfterChange.X, windowRecAfterChange.Y);
                    var @event = new WindowMovementEvent
                    {
                        IssuingModule = WindowManagementModule.Identifier,
                        OldLocation = oldLocation,
                        NewLocation = newLocation,
                        Title = nativeWindowManagement.GetWindowTitleFromHwnd(msg.Hwnd),
                        ProcessName = nativeWindowManagement.GetProcessNameFromHwnd(msg.Hwnd)
                    };
                    Enqueue(@event);
                }
            }
        }
    }
}