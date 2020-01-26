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
        ///     The low level mouse MouseHookHandle
        /// </summary>
        private IntPtr MouseHookHandle;

        public void StartCapture()
        {
            callback = MouseHookCallback; // Store callback to prevent GC
            if (!NativeMethods.TrySetMouseHook(callback, out MouseHookHandle))
            {
                throw new Exception("Failed hook mouse.");
            }
        }

        public void StopCapture()
        {
            if (!NativeMethods.UnhookWindowsHookEx(MouseHookHandle))
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

            MouseAction mouseAction;

            if (wParam == NativeMethods.MessageType.WM_LBUTTONDOWN)
            {
                // a left single mouse click is detected
                if (isWaitingOnLeftDoubleClick)
                {
                    // a left double click is expected
                    mouseAction = MouseAction.LeftDoubleClick;
                    // reset isWaitingOnLeftDoubleClick to false
                    isWaitingOnLeftDoubleClick = false;
                }
                else
                {
                    // a left double click is not expected
                    mouseAction = MouseAction.LeftClick;
                    // set isWaitingOnLeftDoubleClick to false
                    isWaitingOnLeftDoubleClick = true;
                    //start the Timer countdown to wait on a left double click
                    WaitOnDoubleClick(doubleClickTime, ResetIsWaitingOnLeftDoubleClick);
                }
            }
            else if (wParam == NativeMethods.MessageType.WM_RBUTTONDOWN)
            {
                // a right single mouse click is detected
                if (isWaitingOnRightDoubleClick)
                {
                    // a right double click is expected
                    mouseAction = MouseAction.RightDoubleClick;
                    // reset isWaitingOnRightDoubleClick to false
                    isWaitingOnRightDoubleClick = false;
                }
                else
                {
                    // a right double click is not expected
                    mouseAction = MouseAction.RightClick;
                    // set isWaitingOnRightDoubleClick to false
                    isWaitingOnRightDoubleClick = true;
                    //start the Timer countdown to wait on a right double click
                    WaitOnDoubleClick(doubleClickTime, ResetIsWaitingOnRightDoubleClick);
                }
            }
            else if (wParam == NativeMethods.MessageType.WM_MBUTTONDOWN)
            {
                // a middle single mouse click is detected
                if (isWaitingOnMiddleDoubleClick)
                {
                    // a middle double click is expected
                    mouseAction = MouseAction.MiddleDoubleClick;
                    // reset isWaitingOnMiddleDoubleClick to false
                    isWaitingOnMiddleDoubleClick = false;
                }
                else
                {
                    // a middle double click is not expected
                    mouseAction = MouseAction.MiddleClick;
                    // set isWaitingOnMiddleDoubleClick to false
                    isWaitingOnMiddleDoubleClick = true;
                    //start the Timer countdown to wait on a middle double click
                    WaitOnDoubleClick(doubleClickTime, ResetIsWaitingOnMiddleDoubleClick);
                }
            }
            else
            {
                // no click is detected
                mouseAction = MouseAction.None;
            }

            //TODO get the Intptr of the window
            IntPtr hwnd = IntPtr.Zero;

            if (mouseAction != MouseAction.None)
            {
                // create a MouseClickEvent
                var @event = new MouseClickEvent(){MouseAction = mouseAction,MousePosition = lParam.pt, HWnd = hwnd};



                //Enqueue the MouseClickEvent
                Enqueue(@event);
            }

            return NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        /// <summary>
        ///     Waits on a double click
        ///     Starts a timer countdown with given due time
        ///     The callback method will be called when the due time is reached
        /// </summary>
        /// <param name="dueTime">the due time in milliseconds</param>
        /// <param name="callback">the Timer callback method</param>
        public void WaitOnDoubleClick(uint dueTime, TimerCallback callback)
        {
            var t = new Timer(callback);
            t.Change(dueTime, 0);
        }

        /// <summary>
        ///     This method will be called by StartTimer() method to
        ///     reset the isWaitingOnLeftDoubleClick to false.
        /// </summary>
        /// <param name="state">
        ///     An object containing application-specific information relevant to the method invoked by this
        ///     delegate, or null.
        /// </param>
        private void ResetIsWaitingOnLeftDoubleClick(object state)
        {
            isWaitingOnLeftDoubleClick = true;
            // The state object is the Timer object.
            var t = (Timer) state;
            t.Dispose();
            isWaitingOnLeftDoubleClick = false;
        }

        /// <summary>
        ///     This method will be called by StartTimer() method to
        ///     reset the isWaitingOnRightDoubleClick to false and dispose the timer
        /// </summary>
        /// <param name="state">
        ///     An object containing application-specific information relevant to the method invoked by this
        ///     delegate, or null.
        /// </param>
        private void ResetIsWaitingOnRightDoubleClick(object state)
        {
            // The state object is the Timer object.
            var t = (Timer) state;
            t.Dispose();
            isWaitingOnRightDoubleClick = false;
        }

        /// <summary>
        ///     This method will be called by StartTimer() method to
        ///     reset the isWaitingOnMiddleDoubleClick to false and dispose the timer.
        /// </summary>
        /// <param name="state">
        ///     An object containing application-specific information relevant to the method invoked by this
        ///     delegate, or null.
        /// </param>
        private void ResetIsWaitingOnMiddleDoubleClick(object state)
        {
            // The state object is the Timer object.
            var t = (Timer) state;
            t.Dispose();
            isWaitingOnMiddleDoubleClick = false;
        }
    }
}