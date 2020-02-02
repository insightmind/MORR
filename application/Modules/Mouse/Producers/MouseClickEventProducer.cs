using System.Windows;
using System.Windows.Input;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseClickEvent
    /// </summary>
    public class MouseClickEventProducer : DefaultEventQueue<MouseClickEvent>
    {
        public void StartCapture()
        {
            GlobalHook.AddListener(MouseHookCallback, NativeMethods.MessageType.WM_RBUTTONDOWN,
                                   NativeMethods.MessageType.WM_LBUTTONDOWN,
                                   NativeMethods.MessageType.WM_MBUTTONDOWN,
                                   NativeMethods.MessageType.WM_RBUTTONDBLCLK,
                                   NativeMethods.MessageType.WM_LBUTTONDBLCLK,
                                   NativeMethods.MessageType.WM_MBUTTONDBLCLK);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.AddListener(MouseHookCallback, NativeMethods.MessageType.WM_RBUTTONDOWN,
                                   NativeMethods.MessageType.WM_LBUTTONDOWN,
                                   NativeMethods.MessageType.WM_MBUTTONDOWN,
                                   NativeMethods.MessageType.WM_RBUTTONDBLCLK,
                                   NativeMethods.MessageType.WM_LBUTTONDBLCLK,
                                   NativeMethods.MessageType.WM_MBUTTONDBLCLK);
        }

        private void MouseHookCallback(GlobalHook.HookMessage hookMessage)
        {
            var messageType = (NativeMethods.MessageType) hookMessage.Type;
            var mouseAction = GetMouseAction(messageType);
            if (mouseAction != MouseAction.None)
            {
                var mousePosition = new Point { X = hookMessage.Data[0], Y = hookMessage.Data[1] };
                var hwnd = hookMessage.Hwnd.ToString();
                var @event = new MouseClickEvent
                    { MouseAction = mouseAction, MousePosition = mousePosition, HWnd = hwnd };
                Enqueue(@event);
            }
        }


        private MouseAction GetMouseAction(NativeMethods.MessageType messageType)
        {
            if (messageType == NativeMethods.MessageType.WM_RBUTTONDOWN)
            {
                return MouseAction.RightClick;
            }

            if (messageType == NativeMethods.MessageType.WM_LBUTTONDOWN)
            {
                return MouseAction.LeftClick;
            }

            if (messageType == NativeMethods.MessageType.WM_MBUTTONDOWN)
            {
                return MouseAction.MiddleClick;
            }

            if (messageType == NativeMethods.MessageType.WM_RBUTTONDBLCLK)
            {
                return MouseAction.RightDoubleClick;
            }

            if (messageType == NativeMethods.MessageType.WM_LBUTTONDBLCLK)
            {
                return MouseAction.LeftDoubleClick;
            }

            if (messageType == NativeMethods.MessageType.WM_MBUTTONDBLCLK)
            {
                return MouseAction.MiddleDoubleClick;
            }

            return MouseAction.None;
        }
    }
}