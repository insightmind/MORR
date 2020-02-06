using System.Drawing;
using MORR.Modules.WindowManagement.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowResizingEvent
    /// </summary>
    public class WindowResizingEventProducer : DefaultEventQueue<WindowResizingEvent>
    {
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
            GlobalHook.AddListener(WindowHookCallback, NativeMethods.MessageType.WM_ENTERSIZEMOVE,
                                   NativeMethods.MessageType.WM_EXITSIZEMOVE);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(WindowHookCallback, NativeMethods.MessageType.WM_ENTERSIZEMOVE,
                                      NativeMethods.MessageType.WM_EXITSIZEMOVE);
        }

        /// <summary>
        ///     Everytime a WM_ENTERSIZEMOVE is received,
        ///     records the information of the window before the change
        ///     and wait for the WM_EXITSIZEMOVE.
        ///     Everytime a WM_EXITSIZEMOVE is received,
        ///     records the information of the window after the change
        ///     and see if the window has been resized.
        ///     (by if !(Utility.IsRectSizeEqual(windowRecBeforeChange,windowRecAfterChange)))
        ///     If so, records the relevant information into a WindowResizingEvent.
        /// </summary>
        /// <param name="msg">the hook message</param>
        private void WindowHookCallback(GlobalHook.HookMessage msg)
        {
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
                    var oldSize = new Size
                    {
                        Width = Utility.GetWindowWidth(windowRecBeforeChange),
                        Height = Utility.GetWindowHeight(windowRecBeforeChange)
                    };
                    var newSize = new Size
                    {
                        Width = Utility.GetWindowWidth(windowRecAfterChange),
                        Height = Utility.GetWindowHeight(windowRecAfterChange)
                    };
                    var @event = new WindowResizingEvent
                    {
                        IssuingModule = WindowManagementModule.Identifier,
                        OldSize = oldSize,
                        NewSize = newSize,
                        Title = Utility.GetWindowTitleFromHwnd(msg.Hwnd),
                        ProcessName = Utility.GetProcessNameFromHwnd(msg.Hwnd)
                    };
                    Enqueue(@event);
                }
            }
        }
    }
}