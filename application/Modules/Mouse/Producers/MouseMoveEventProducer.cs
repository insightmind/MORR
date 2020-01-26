using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseMoveEvent
    /// </summary>
    [Export(typeof(MouseMoveEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<MouseMoveEvent>))]
    [Export(typeof(IReadWriteEventQueue<MouseMoveEvent>))]
    [Export(typeof(IReadOnlyEventQueue<Event>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MouseMoveEventProducer : DefaultEventQueue<MouseMoveEvent>
    {
        /// <summary>
        ///     The mouse position in the last period.
        ///     This field will be initialized to the mouse position
        ///     when the StartTimer() is called.
        /// </summary>
        private NativeMethods.POINT lastMousePosition;

        /// <summary>
        ///     A timer that records the mouse position at specific intervals.
        /// </summary>
        private Timer mousePositionRecordingTimer;

        /// <summary>
        ///     The time interval between invocation of method to record mouse position, in milliseconds.
        /// </summary>
        private int period { get; set; }

        /// <summary>
        ///     The minimal distance a mouse move must reach in a period to be recorded.
        ///     (A mouse move with distance less than the threshold will be ignored,
        ///     in other words, a new MouseMoveEvent will not be generated and
        ///     the mouse position will not be recorded)
        /// </summary>
        private int threshold { get; set; }

        #region private methods

        /// <summary>
        ///     Get the mouse position and create & enqueue corresponding event
        ///     if the threshold is reached.
        /// </summary>
        /// <param name="stateInfo"></param>
        private void GetMousePosition(object stateInfo)
        {
            // get the current mouse position as Point
            NativeMethods.POINT currentMousePosition;
            NativeMethods.GetCursorPos(out currentMousePosition);

            // compare the last and the current mouse position and compute their distance
            var distance = Point.Subtract(lastMousePosition, currentMousePosition).Length;

            // replace the last mouse position with the current mouse position
            lastMousePosition = currentMousePosition;

            //if the distance that the mouse has been moved reaches(is greater than) the threshold
            //record the new Position in the created MouseMoveEvent and enqueue it
            if (distance >= threshold)
            {
                var @event = new MouseMoveEvent() { MousePosition = currentMousePosition};
                Enqueue(@event);
            }
        }

        #endregion


        #region public methods

        public void Configure(int period, int threshold)
        {
            this.period = period;
            this.threshold = threshold;
        }

        /// <summary>
        ///     start the mouse movement capture by starting the timer that records mouse position.
        /// </summary>
        public void StartCapture()
        {
            NativeMethods.GetCursorPos(out lastMousePosition);

            mousePositionRecordingTimer = new Timer(GetMousePosition, null, 0, period);
        }

        /// <summary>
        ///     stop the mouse movement capture by disposing the timer that records mouse position.
        /// </summary>
        public void StopCapture()
        {
            mousePositionRecordingTimer.Dispose();
        }

        #endregion
    }
}