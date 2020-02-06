using System.Windows;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseScrollEvent
    /// </summary>
    public class MouseScrollEventProducer : DefaultEventQueue<MouseScrollEvent>
    {
        public void StartCapture()
        {
            GlobalHook.AddListener(MouseHookCallback, NativeMethods.MessageType.WM_MOUSEWHEEL);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(MouseHookCallback, NativeMethods.MessageType.WM_MOUSEWHEEL);
            NotifyOnEnqueueFinished();
        }

        private void MouseHookCallback(GlobalHook.HookMessage hookMessage)
        {
            //One wheel click is defined as WHEEL_DELTA, which is 120. 
            //The scroll amount is the high order word of the wParam
            var highOrderWord = (hookMessage.wParam.ToInt64() >> 16) & 0xffff;
            var scrollAmount = (short)highOrderWord;
            var mousePosition = new Point { X = hookMessage.Data[0], Y = hookMessage.Data[1] };
            var hwnd = hookMessage.Hwnd.ToString();
            var @event = new MouseScrollEvent
                { ScrollAmount = scrollAmount, MousePosition = mousePosition, HWnd = hwnd, IssuingModule = MouseModule.Identifier};
            Enqueue(@event);
        }
    }
}