using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Input;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseClickEvent
    /// </summary>
    [Export(typeof(MouseClickEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<MouseClickEvent>))]
    [Export(typeof(IReadWriteEventQueue<MouseClickEvent>))]
    [Export(typeof(IReadOnlyEventQueue<Event>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MouseClickEventProducer : DefaultEventQueue<MouseClickEvent>
    {
        /// <summary>
        ///     the maximum number of milliseconds that may occur between the first and
        ///     second click of a double-click.
        /// </summary>
        private readonly uint doubleClickTime = NativeMethods.GetDoubleClickTime();

        private NativeMethods.LowLevelMouseProc? callback;

        /// <summary>
        ///     true if a left single click is detected and a left double click is expected
        ///     in the double click time
        ///     false if no left single click is detected or a left single click is detected but the
        ///     double click time is passed
        ///     this field should be initialized to false because click detecting happens only after the initialization
        /// </summary>
        private bool isWaitingOnLeftDoubleClick;

        /// <summary>
        ///     true if a middle single click is detected and a middle double click is expected
        ///     in the double click time
        ///     false if no middle single click is detected or a middle single click is detected but the
        ///     double click time is passed
        ///     this field should be initialized to false because click detecting happens only after the initialization
        /// </summary>
        private bool isWaitingOnMiddleDoubleClick;

        /// <summary>
        ///     true if a right single click is detected and a right double click is expected
        ///     in the double click time
        ///     false if no right single click is detected or a right single click is detected but the
        ///     double click time is passed
        ///     this field should be initialized to false because click detecting happens only after the initialization
        /// </summary>
        private bool isWaitingOnRightDoubleClick;

        /// <summary>
        ///     The low level mouse mouseHookHandle
        /// </summary>
        private IntPtr mouseHookHandle;

        public void StartCapture()
        {
            callback = MouseHookCallback; // Store callback to prevent GC
            if (!NativeMethods.TrySetMouseHook(callback, out mouseHookHandle))
            {
                throw new Exception("Failed hook mouse.");
            }
        }

        public void StopCapture()
        {
            if (!NativeMethods.UnhookWindowsHookEx(mouseHookHandle))
            {
                throw new Exception("Failed to unhook mouse.");
            }
        }


        private int MouseHookCallback(int nCode,
                                      NativeMethods.MessageType wParam,
                                      NativeMethods.MSLLHOOKSTRUCT lParam)
        {
            if (nCode < 0)
            {
                // Required as per documentation
                // see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644985(v=vs.85)#return-value
                return NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            }

            var mouseAction = GetMouseAction(wParam);
            if (mouseAction != MouseAction.None)
            {
                //TODO get the Intptr of the window
                var hwnd = IntPtr.Zero;

                var @event = new MouseClickEvent { MouseAction = mouseAction, MousePosition = lParam.pt, HWnd = hwnd };
                Enqueue(@event);
            }

            return NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }


        /// <summary>
        ///     Returns the MouseAction base on the wParam
        /// </summary>
        /// <param name="wParam"></param>
        /// <returns></returns>
        private MouseAction GetMouseAction(NativeMethods.MessageType wParam)
        {
            if (wParam == NativeMethods.MessageType.WM_LBUTTONDOWN)
            {
                if (isWaitingOnLeftDoubleClick)
                {
                    isWaitingOnLeftDoubleClick = false;
                    return MouseAction.LeftDoubleClick;
                }

                isWaitingOnLeftDoubleClick = true;
                WaitOnDoubleClick(doubleClickTime, isWaitingOnLeftDoubleClick);
                return MouseAction.LeftClick;
            }

            if (wParam == NativeMethods.MessageType.WM_RBUTTONDOWN)
            {
                if (isWaitingOnRightDoubleClick)
                {
                    isWaitingOnRightDoubleClick = false;
                    return MouseAction.RightDoubleClick;
                }

                isWaitingOnRightDoubleClick = true;
                WaitOnDoubleClick(doubleClickTime, isWaitingOnRightDoubleClick);
                return MouseAction.RightClick;
            }

            if (wParam == NativeMethods.MessageType.WM_MBUTTONDOWN)
            {
                if (isWaitingOnMiddleDoubleClick)
                {
                    isWaitingOnMiddleDoubleClick = false;
                    return MouseAction.MiddleDoubleClick;
                }

                isWaitingOnMiddleDoubleClick = true;
                WaitOnDoubleClick(doubleClickTime, isWaitingOnMiddleDoubleClick);
                return MouseAction.MiddleClick;
            }

            return MouseAction.None;
        }


        /// <summary>
        ///     Starts a timer with which a isWaitingOnDouble~Click Boolean type will be reset
        ///     to false if the due time is reached. Before the due time is reached, a potential
        ///     double click is expected
        /// </summary>
        /// <param name="dueTime">due time in milliseconds</param>
        /// <param name="isWaitingOnDoubleClick">a Boolean indicates if it is waiting for a certain type of double click</param>
        private void WaitOnDoubleClick(uint dueTime, bool isWaitingOnDoubleClick)
        {
            var timer = new Timer(ResetIsWaitingOnDoubleClick, isWaitingOnDoubleClick, dueTime, 0);
            timer.Dispose();
        }

        /// <summary>
        ///     This method will be called by WaitOnDoubleClick() method to
        ///     reset the isWaitingOn~DoubleClick to false.
        /// </summary>
        /// <param name="isWaitingOnDoubleClick">a Boolean indicates if it is waiting for a certain type of double click</param>
        private void ResetIsWaitingOnDoubleClick(object isWaitingOnDoubleClick)
        {
            isWaitingOnDoubleClick = false;
        }
    }
}