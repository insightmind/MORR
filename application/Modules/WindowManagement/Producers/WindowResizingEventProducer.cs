using System.Drawing;
using MORR.Modules.WindowManagement.Events;
using MORR.Modules.WindowManagement.Native;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowResizingEvent
    /// </summary>
    public class WindowResizingEventProducer : DefaultEventQueue<WindowResizingEvent>
    {
        private static readonly INativeWindowManagement nativeWindowManagement = new NativeWindowManagement();

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
            const int dataParamTest = 3;
            if (msg.Type == (uint) GlobalHook.MessageType.WM_ENTERSIZEMOVE)
            {
                windowUnderChangeHwnd = (int) msg.Hwnd;
                windowRecBeforeChange = new Rectangle();
                nativeWindowManagement.GetWindowRect(windowUnderChangeHwnd, ref windowRecBeforeChange);
            }

            if (msg.Type == (uint) GlobalHook.MessageType.WM_EXITSIZEMOVE)
            {
                if (msg.Data[0] == dataParamTest)
                {
                    var @event = new WindowResizingEvent
                    {
                        IssuingModule = WindowManagementModule.Identifier,
                        ProcessName = "sampleProcessName",
                        Title = "sampleResizingTitle",
                        OldSize = new Size(0, 0),
                        NewSize = new Size(1, 1)
                    };
                    Enqueue(@event);
                    return;
                }

                windowRecAfterChange = new Rectangle();
                nativeWindowManagement.GetWindowRect(windowUnderChangeHwnd, ref windowRecAfterChange);
                if (!nativeWindowManagement.IsRectSizeEqual(windowRecBeforeChange, windowRecAfterChange))
                {
                    var oldSize = new Size
                    {
                        Width = nativeWindowManagement.GetWindowWidth(windowRecBeforeChange),
                        Height = nativeWindowManagement.GetWindowHeight(windowRecBeforeChange)
                    };
                    var newSize = new Size
                    {
                        Width = nativeWindowManagement.GetWindowWidth(windowRecAfterChange),
                        Height = nativeWindowManagement.GetWindowHeight(windowRecAfterChange)
                    };
                    var @event = new WindowResizingEvent
                    {
                        IssuingModule = WindowManagementModule.Identifier,
                        OldSize = oldSize,
                        NewSize = newSize,
                        Title = nativeWindowManagement.GetWindowTitleFromHwnd(msg.Hwnd),
                        ProcessName = nativeWindowManagement.GetProcessNameFromHwnd(msg.Hwnd)
                    };
                    Enqueue(@event);
                }
            }
        }
    }
}