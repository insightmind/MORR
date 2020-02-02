using System;
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
                                   NativeMethods.MessageType.WM_MBUTTONDBLCLK,
                                   NativeMethods.MessageType.WM_NCRBUTTONDOWN,
                                   NativeMethods.MessageType.WM_NCLBUTTONDOWN,
                                   NativeMethods.MessageType.WM_NCMBUTTONDOWN,
                                   NativeMethods.MessageType.WM_NCRBUTTONDBLCLK,
                                   NativeMethods.MessageType.WM_NCLBUTTONDBLCLK,
                                   NativeMethods.MessageType.WM_NCMBUTTONDBLCLK);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(MouseHookCallback, NativeMethods.MessageType.WM_RBUTTONDOWN,
                                      NativeMethods.MessageType.WM_LBUTTONDOWN,
                                      NativeMethods.MessageType.WM_MBUTTONDOWN,
                                      NativeMethods.MessageType.WM_RBUTTONDBLCLK,
                                      NativeMethods.MessageType.WM_LBUTTONDBLCLK,
                                      NativeMethods.MessageType.WM_MBUTTONDBLCLK,
                                      NativeMethods.MessageType.WM_NCRBUTTONDOWN,
                                      NativeMethods.MessageType.WM_NCLBUTTONDOWN,
                                      NativeMethods.MessageType.WM_NCMBUTTONDOWN,
                                      NativeMethods.MessageType.WM_NCRBUTTONDBLCLK,
                                      NativeMethods.MessageType.WM_NCLBUTTONDBLCLK,
                                      NativeMethods.MessageType.WM_NCMBUTTONDBLCLK);
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
                { MouseAction = mouseAction, MousePosition = mousePosition, HWnd = hwnd, IssuingModule = MouseModule.Identifier };
                Enqueue(@event);
            }
        }


        private MouseAction GetMouseAction(NativeMethods.MessageType messageType)
        {
            return messageType switch
            {
                NativeMethods.MessageType.WM_RBUTTONDOWN => MouseAction.RightClick,
                NativeMethods.MessageType.WM_NCRBUTTONDOWN => MouseAction.RightClick,
                NativeMethods.MessageType.WM_LBUTTONDOWN => MouseAction.LeftClick,
                NativeMethods.MessageType.WM_NCLBUTTONDOWN => MouseAction.LeftClick,
                NativeMethods.MessageType.WM_MBUTTONDOWN => MouseAction.MiddleClick,
                NativeMethods.MessageType.WM_NCMBUTTONDOWN => MouseAction.MiddleClick,
                NativeMethods.MessageType.WM_RBUTTONDBLCLK => MouseAction.RightDoubleClick,
                NativeMethods.MessageType.WM_NCRBUTTONDBLCLK => MouseAction.RightDoubleClick,
                NativeMethods.MessageType.WM_LBUTTONDBLCLK => MouseAction.LeftDoubleClick,
                NativeMethods.MessageType.WM_NCLBUTTONDBLCLK => MouseAction.LeftDoubleClick,
                NativeMethods.MessageType.WM_MBUTTONDBLCLK => MouseAction.MiddleDoubleClick,
                NativeMethods.MessageType.WM_NCMBUTTONDBLCLK => MouseAction.MiddleDoubleClick,
                _ => MouseAction.None
            };
        }
    }
}