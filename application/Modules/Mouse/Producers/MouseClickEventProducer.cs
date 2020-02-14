using System.Windows;
using System.Windows.Input;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseClickEvent
    /// </summary>
    public class MouseClickEventProducer : DefaultEventQueue<MouseClickEvent>
    {
        private readonly GlobalHook.MessageType[] listenedMessagesTypes = 
        {
            GlobalHook.MessageType.WM_RBUTTONDOWN,
            GlobalHook.MessageType.WM_LBUTTONDOWN,
            GlobalHook.MessageType.WM_MBUTTONDOWN,
            GlobalHook.MessageType.WM_RBUTTONDBLCLK,
            GlobalHook.MessageType.WM_LBUTTONDBLCLK,
            GlobalHook.MessageType.WM_MBUTTONDBLCLK,
            GlobalHook.MessageType.WM_NCRBUTTONDOWN,
            GlobalHook.MessageType.WM_NCLBUTTONDOWN,
            GlobalHook.MessageType.WM_NCMBUTTONDOWN,
            GlobalHook.MessageType.WM_NCRBUTTONDBLCLK,
            GlobalHook.MessageType.WM_NCLBUTTONDBLCLK,
            GlobalHook.MessageType.WM_NCMBUTTONDBLCLK
        };

        public void StartCapture()
        {
            GlobalHook.AddListener(MouseHookCallback, listenedMessagesTypes);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(MouseHookCallback, listenedMessagesTypes);
            Close();
        }

        private void MouseHookCallback(GlobalHook.HookMessage hookMessage)
        {
            var messageType = (GlobalHook.MessageType) hookMessage.Type;
            var mouseAction = GetMouseAction(messageType);
            if (mouseAction == MouseAction.None)
            {
                return;
            }

            var mousePosition = new Point { X = hookMessage.Data[0], Y = hookMessage.Data[1] };
            var hwnd = hookMessage.Hwnd.ToString();
            var @event = new MouseClickEvent { MouseAction = mouseAction, MousePosition = mousePosition, HWnd = hwnd, IssuingModule = MouseModule.Identifier };
            Enqueue(@event);
        }


        private MouseAction GetMouseAction(GlobalHook.MessageType messageType)
        {
            return messageType switch
            {
                GlobalHook.MessageType.WM_RBUTTONDOWN => MouseAction.RightClick,
                GlobalHook.MessageType.WM_NCRBUTTONDOWN => MouseAction.RightClick,
                GlobalHook.MessageType.WM_LBUTTONDOWN => MouseAction.LeftClick,
                GlobalHook.MessageType.WM_NCLBUTTONDOWN => MouseAction.LeftClick,
                GlobalHook.MessageType.WM_MBUTTONDOWN => MouseAction.MiddleClick,
                GlobalHook.MessageType.WM_NCMBUTTONDOWN => MouseAction.MiddleClick,
                GlobalHook.MessageType.WM_RBUTTONDBLCLK => MouseAction.RightDoubleClick,
                GlobalHook.MessageType.WM_NCRBUTTONDBLCLK => MouseAction.RightDoubleClick,
                GlobalHook.MessageType.WM_LBUTTONDBLCLK => MouseAction.LeftDoubleClick,
                GlobalHook.MessageType.WM_NCLBUTTONDBLCLK => MouseAction.LeftDoubleClick,
                GlobalHook.MessageType.WM_MBUTTONDBLCLK => MouseAction.MiddleDoubleClick,
                GlobalHook.MessageType.WM_NCMBUTTONDBLCLK => MouseAction.MiddleDoubleClick,
                _ => MouseAction.None
            };
        }
    }
}