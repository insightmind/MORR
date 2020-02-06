using System.Drawing;
using MORR.Modules.WindowManagement.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowMovementEvent
    /// </summary>
    public class WindowMovementEventProducer : DefaultEventQueue<WindowMovementEvent>
    {
        private int windowUnderChangeHwnd;
        private Rectangle windowRecBeforeChange;
        private Rectangle windowRecAfterChange;

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

        private void WindowHookCallback(GlobalHook.HookMessage msg)
        {
            //Console.WriteLine("Received event type {0} from hwnd {1} and wParam {2} and the new x : {3}, new y: {4}", msg.Type, msg.Hwnd, msg.wParam,msg.Data[0],msg.Data[1]);
            if (msg.Type == (uint)NativeMethods.MessageType.WM_ENTERSIZEMOVE)
            {
                windowUnderChangeHwnd = (int)msg.Hwnd;
                windowRecBeforeChange = new Rectangle();
                NativeMethods.GetWindowRect(windowUnderChangeHwnd, ref windowRecBeforeChange);
            }
            if (msg.Type == (uint)NativeMethods.MessageType.WM_EXITSIZEMOVE)
            {
                windowRecAfterChange = new Rectangle();
                NativeMethods.GetWindowRect(windowUnderChangeHwnd, ref windowRecAfterChange);
                if (Utility.IsRectSizeEqual(windowRecBeforeChange,windowRecAfterChange))
                {
                    System.Windows.Point oldLocation = new System.Windows.Point { X = windowRecBeforeChange.X, Y = windowRecBeforeChange.Y };
                    System.Windows.Point newLocation = new System.Windows.Point { X = windowRecAfterChange.X, Y = windowRecAfterChange.Y };
                    WindowMovementEvent @event = new WindowMovementEvent {IssuingModule = WindowManagementModule.Identifier,
                                                                          OldLocation = oldLocation,
                                                                          NewLocation = newLocation,
                                                                          Title = Utility.GetWindowTitleFromHwnd(msg.Hwnd),
                                                                          ProcessName = Utility.GetProcessNameFromHwnd(msg.Hwnd)};
                    Enqueue(@event);
                }                                                         
            }
        }
    }
}