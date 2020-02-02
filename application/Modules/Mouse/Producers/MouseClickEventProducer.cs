using System.Threading;
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
        private readonly ClickInformationAndState leftClickIS = new ClickInformationAndState
        {
            MessageType = NativeMethods.MessageType.WM_LBUTTONDOWN, SingleClickAction = MouseAction.LeftClick,
            DoubleClickAction = MouseAction.LeftDoubleClick
        };

        private readonly ClickInformationAndState middleClickIS = new ClickInformationAndState
        {
            MessageType = NativeMethods.MessageType.WM_MBUTTONDOWN, SingleClickAction = MouseAction.MiddleClick,
            DoubleClickAction = MouseAction.MiddleDoubleClick
        };

        private readonly ClickInformationAndState rightClickIS = new ClickInformationAndState
        {
            MessageType = NativeMethods.MessageType.WM_RBUTTONDOWN, SingleClickAction = MouseAction.RightClick,
            DoubleClickAction = MouseAction.RightDoubleClick
        };

        public void StartCapture()
        {
            GlobalHook.AddListener(MouseHookCallback, leftClickIS.MessageType,
                                   rightClickIS.MessageType, middleClickIS.MessageType);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(MouseHookCallback, leftClickIS.MessageType,
                                   rightClickIS.MessageType, middleClickIS.MessageType);
        }


        private void MouseHookCallback(GlobalHook.HookMessage hookMessage)
        {
            var messageType = (NativeMethods.MessageType) hookMessage.Type;
            var mouseAction = GetMouseAction(messageType);
            if (mouseAction != MouseAction.None)
            {
                var mousePosition = new Point { X = hookMessage.Data[0], Y = hookMessage.Data[1] };
                var hwnd = hookMessage.Hwnd;
                var @event = new MouseClickEvent
                    { MouseAction = mouseAction, MousePosition = mousePosition, HWnd = hwnd };
                Enqueue(@event);
            }
        }


        private MouseAction GetMouseAction(NativeMethods.MessageType wParam)
        {
            if (wParam == leftClickIS.MessageType)
            {
                return DetectAndGetSingleOrDoubleClick(leftClickIS);
            }

            if (wParam == rightClickIS.MessageType)
            {
                return DetectAndGetSingleOrDoubleClick(rightClickIS);
            }

            if (wParam == middleClickIS.MessageType)
            {
                return DetectAndGetSingleOrDoubleClick(middleClickIS);
            }

            return MouseAction.None;
        }

        private MouseAction DetectAndGetSingleOrDoubleClick(ClickInformationAndState clickIS)
        {
            if (clickIS.isExpectingDoubleClick)
            {
                StopExpectingDoubleClick(clickIS);
                return clickIS.DoubleClickAction;
            }

            StartExpectingDoubleClick(clickIS);
            return clickIS.SingleClickAction;
        }

        /// <summary>
        ///     Start expecting a potential double click.
        ///     Implementation detail:
        ///     Starts a timer with which the isExpectingDoubleClick of
        ///     the given ClickInformationAndState object will be reset
        ///     to false if the double click time is reached.
        ///     Before that, a potential double click is expected.
        ///     The double click time is defined as:
        ///     the maximum number of milliseconds that may occur between
        ///     the first and second click of a double-click.
        /// </summary>
        /// <param name="clickIS">A class containing information about a certain click type</param>
        private void StartExpectingDoubleClick(ClickInformationAndState clickIS)
        {
            clickIS.isExpectingDoubleClick = true;
            var timer = new Timer(StopExpectingDoubleClick, clickIS, NativeMethods.GetDoubleClickTime(), 0);
            timer.Dispose();
        }

        /// <summary>
        ///     This method will be called by StartExpectingDoubleClick() after the double click time method to
        ///     reset the isExpectingDoubleClick field in the ClickInformationAndState object to false
        ///     so that a double click will not be expected after the double click time.
        /// </summary>
        /// <param name="stateObject">
        ///     the state object for the timer callback, in this case, an object of type
        ///     ClickInformationAndState
        /// </param>
        private void StopExpectingDoubleClick(object stateObject)
        {
            var clickIS = (ClickInformationAndState) stateObject;
            clickIS.isExpectingDoubleClick = false;
        }

        /// <summary>
        ///     A class containing information about a certain click type
        ///     including left, right and middle click.
        ///     The click type does not differentiate between single or double click.
        /// </summary>
        private class ClickInformationAndState
        {
            /// true if a single click is detected and a double click is expected
            /// in the double click time
            /// false if no single click is detected or a single click is detected but the
            /// double click time has passed
            public bool isExpectingDoubleClick;

            /// <summary>
            ///     The Windows message that corresponds to the type of the click type
            /// </summary>
            public NativeMethods.MessageType MessageType;

            /// <summary>
            ///     The single click MouseAction of the click type
            /// </summary>
            public MouseAction SingleClickAction;

            /// <summary>
            ///     The double click MouseAction of the click type
            /// </summary>
            public MouseAction DoubleClickAction;
        }
    }
}