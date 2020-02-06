using System;
using System.ComponentModel.Composition;
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
        private int windowUnderChangeHwnd;
        private Rectangle windowRecBeforeChange;
        private Rectangle windowRecAfterChange;

        public void StartCapture()
        {
            GlobalHook.AddListener(WindowHookCallback, NativeMethods.MessageType.WM_MOVE,
                                                       NativeMethods.MessageType.WM_ENTERSIZEMOVE,
                                                       NativeMethods.MessageType.WM_EXITSIZEMOVE);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(WindowHookCallback, NativeMethods.MessageType.WM_MOVE,
                                                       NativeMethods.MessageType.WM_ENTERSIZEMOVE,
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
                if (Utility.IsRectSizeEqual(windowRecBeforeChange, windowRecAfterChange))
                {
                    Console.WriteLine("The Window is moved, the old location is x: {0}, y: {1} and the new location is x:{2}, y: {3}",
                                                    windowRecBeforeChange.X,
                                                    windowRecBeforeChange.Y,
                                                    windowRecAfterChange.X,
                                                    windowRecAfterChange.Y);
                }
                else
                {
                    Console.WriteLine("The Window is resized, the old size is {0} * {1} and the new size is x:{2} * {3}",
                                        Utility.GetWindowWidth(windowRecBeforeChange),
                                        Utility.GetWindowHeight(windowRecBeforeChange),
                                        Utility.GetWindowWidth(windowRecAfterChange),
                                        Utility.GetWindowHeight(windowRecAfterChange));
                }
                Console.WriteLine("");
            }
        }
    }
}