using System.Composition;
using System.Threading;
using System.Windows;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseMoveEvent
    /// </summary>
    [Export(typeof(MouseMoveEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<MouseMoveEvent>))]
    public class MouseMoveEventProducer : DefaultEventQueue<MouseMoveEvent>
    {
        #region constructor

        /// <summary>
        ///     Initialize a MouseMoveEventProducer with period and threshold.
        /// </summary>
        /// <param name="period">The time interval between invocation of method to record mouse position, in milliseconds. </param>
        /// <param name="threshold">The minimal distance a mouse move must reach in a period to be recorded.</param>
        public MouseMoveEventProducer(int period, double threshold) : base(new KeepAllStorageStrategy())
        {
            this.period = period;
            this.threshold = threshold;
        }

        #endregion

        #region private methods

        /// <summary>
        ///     Get the mouse position and create & enqueue corresponding event
        ///     if the threshold is reached.
        /// </summary>
        /// <param name="stateInfo"></param>
        private void GetMousePosition(object stateInfo)
        {
            // get the current mouse position as Point
            Point currentMousePosition;
            NativeMethods.GetCursorPos(out currentMousePosition);

            // compare the last and the current mouse position and compute their distance
            var distance = Point.Subtract(lastMousePosition, currentMousePosition).Length;

            // replace the last mouse position with the current mouse position
            lastMousePosition = currentMousePosition;

            //if the distance that the mouse has been moved reaches(is greater than) the threshold
            //record the new Position in the created MouseMoveEvent and enqueue it
            if (distance >= threshold)
            {
                var @event = new MouseMoveEvent();
                @event.MousePosition = currentMousePosition;
                Enqueue(@event);
            }
        }

        #endregion

        #region private fields

        /// <summary>
        ///     The time interval between invocation of method to record mouse position, in milliseconds.
        /// </summary>
        private readonly int period;

        /// <summary>
        ///     The minimal distance a mouse move must reach in a period to be recorded.
        ///     (A mouse move with distance less than the threshold will be ignored,
        ///     in other words, a new MouseMoveEvent will not be generated and
        ///     the mouse position will not be recorded)
        /// </summary>
        private readonly double threshold;

        /// <summary>
        ///     The mouse position in the last period.
        ///     This field will be initialized to the mouse position
        ///     when the StartTimer() is called.
        /// </summary>
        private Point lastMousePosition;

        /// <summary>
        ///     A timer that records the mouse position at specific intervals.
        /// </summary>
        private Timer mousePositionRecordingTimer;

        #endregion

        #region public methods

        /// <summary>
        ///     Start the timer that records mouse position.
        /// </summary>
        public void StartTimer()
        {
            NativeMethods.GetCursorPos(out lastMousePosition);

            mousePositionRecordingTimer = new Timer(GetMousePosition, null, 0, period);
        }

        /// <summary>
        ///     Dispose the timer that records mouse position.
        /// </summary>
        public void DisposeTimer()
        {
            mousePositionRecordingTimer.Dispose();
        }

        #endregion
    }
}