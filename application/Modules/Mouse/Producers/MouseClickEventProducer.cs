using System;
using System.Composition;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseClickEvent
    /// </summary>
    [Export(typeof(MouseClickEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<MouseClickEvent>))]
    public class MouseClickEventProducer : DefaultEventQueue<MouseClickEvent>
    {
        #region constructor

        /// <summary>
        ///     initialize the MouseClickEventProducer.
        ///     initialize the isWaitingOn~DoubleClick fields to false.
        ///     retrieves the current double-click time for the mouse.
        /// </summary>
        public MouseClickEventProducer() : base(new KeepAllStorageStrategy())
        {
            isWaitingOnLeftDoubleClick = false;
            isWaitingOnRightDoubleClick = false;
            isWaitingOnRightDoubleClick = false;
            doubleClickTime = NativeMethods.GetDoubleClickTime();
        }

        #endregion

        #region private fields

        /// <summary>
        ///     The low level mouse hook
        /// </summary>
        private IntPtr hook = IntPtr.Zero;

        /// <summary>
        ///     true if a left single click is detected and a left double click is expected
        ///     in the double click time
        ///     false if no left single click is detected or a left single click is detected but the
        ///     double click time is passed
        /// </summary>
        private bool isWaitingOnLeftDoubleClick;

        /// <summary>
        ///     true if a right single click is detected and a right double click is expected
        ///     in the double click time
        ///     false if no right single click is detected or a right single click is detected but the
        ///     double click time is passed
        /// </summary>
        private bool isWaitingOnRightDoubleClick;

        /// <summary>
        ///     true if a middle single click is detected and a middle double click is expected
        ///     in the double click time
        ///     false if no middle single click is detected or a middle single click is detected but the
        ///     double click time is passed
        /// </summary>
        private bool isWaitingOnMiddleDoubleClick;

        /// <summary>
        ///     the maximum number of milliseconds that may occur between the first and
        ///     second click of a double-click.
        /// </summary>
        private readonly uint doubleClickTime;

        #endregion

        #region public methods

        /// <summary>
        ///     Set the hook for the mouse.
        /// </summary>
        public void HookMouse()
        {
            var currentProcess = Process.GetCurrentProcess();
            var currentModule = currentProcess.MainModule;
            var moduleName = currentModule.ModuleName;
            var moduleHandle = NativeMethods.GetModuleHandle(moduleName);
            hook = NativeMethods.SetWindowsHookEx((int) NativeMethods.HookType.WH_MOUSE_LL, HookProc, moduleHandle,
                                                  0);
        }

        /// <summary>
        ///     Release the hook for the mouse.
        /// </summary>
        public void UnhookMouse()
        {
            NativeMethods.UnhookWindowsHookEx(hook);
        }

        #endregion

        #region private methods

        /// <summary>
        ///     The callback for the Mouse hook
        ///     Create MouseClickEvent when user clicks
        /// </summary>
        /// <param name="nCode">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The mouse event information</param>
        /// <returns></returns>
        private int HookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MouseAction mouseAction;

                if (wParam == (int) NativeMethods.MessageType.WM_LBUTTONDOWN)
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
                else if (wParam == (int) NativeMethods.MessageType.WM_RBUTTONDOWN)
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
                else if (wParam == (int) NativeMethods.MessageType.WM_MBUTTONDOWN)
                {
                    // a middle single mouse click is detected
                    if (isWaitingOnRightDoubleClick)
                    {
                        // a middle double click is expected
                        mouseAction = MouseAction.MiddleDoubleClick;
                        // reset isWaitingOnMiddleDoubleClick to false
                        isWaitingOnRightDoubleClick = false;
                    }
                    else
                    {
                        // a middle double click is not expected
                        mouseAction = MouseAction.MiddleClick;
                        // set isWaitingOnMiddleDoubleClick to false
                        isWaitingOnRightDoubleClick = true;
                        //start the Timer countdown to wait on a middle double click
                        WaitOnDoubleClick(doubleClickTime, ResetIsWaitingOnMiddleDoubleClick);
                    }
                }
                else
                {
                    // no click is detected
                    mouseAction = MouseAction.None;
                }


                if (mouseAction != MouseAction.None)
                {
                    // create a MouseClickEvent
                    var @event = new MouseClickEvent();

                    // set the MouseAction
                    @event.MouseAction = mouseAction;

                    //retrieve the mouse position from the lParam
                    var hookStruct =
                        (NativeMethods.MouseHookStruct) Marshal.PtrToStructure(
                            lParam, typeof(NativeMethods.MouseHookStruct));
                    var mousePosition = hookStruct.pt;

                    // set the mouse position
                    @event.MousePosition = mousePosition;

                    //TODO get the Intptr of the window

                    //Enqueue the MouseClickEvent
                    Enqueue(@event);
                }

                var x = NativeMethods.GetDoubleClickTime();
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

        #endregion
    }
}