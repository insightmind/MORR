using System.Threading;
using System.Windows;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseMoveEvent
    /// </summary>
    public class MouseMoveEventProducer : DefaultEventQueue<MouseMoveEvent>
    {
        /// <summary>
        ///     The mouse position in screen coordinates in the last period.
        ///     This field will be initialized to the mouse position
        ///     when the StartTimer() is called.
        /// </summary>
        private NativeMethods.POINT lastMousePosition;

        /// <summary>
        ///     A timer that records the mouse position at specific intervals.
        /// </summary>
        private Timer? mousePositionRecordingTimer;

        /// <summary>
        ///     The sampling rate of the mouse position capture, in Hz.
        /// </summary>
        internal uint SamplingRateInHz { get; set; }

        /// <summary>
        ///     The minimal distance(computed with screen coordinates) a mouse move
        ///     must reach in a period to be recorded.
        ///     A mouse move with distance less than the Threshold will be ignored,
        ///     in other words, a new MouseMoveEvent will not be generated and
        ///     the mouse position will not be recorded.
        /// </summary>
        internal int Threshold { get; set; }

        /// <summary>
        ///     Get the mouse position and create & enqueue corresponding event
        ///     if the Threshold is reached.
        /// </summary>
        /// <param name="stateInfo">state object</param>
        private void GetMousePosition(object stateInfo)
        {
            // get the current mouse position as Point
            NativeMethods.GetCursorPos(out var currentMousePosition);

            var currentMousePositionAsPoint = new Point(currentMousePosition.X, currentMousePosition.Y);
            var lastMousePositionAsPoint = new Point(lastMousePosition.X, lastMousePosition.Y);
            // compare the last and the current mouse position and compute their distance
            var distance = Point.Subtract(lastMousePositionAsPoint, currentMousePositionAsPoint).Length;

            // replace the last mouse position with the current mouse position
            lastMousePosition = currentMousePosition;

            //if the distance that the mouse has been moved reaches(is greater than) the Threshold
            //record the new Position in the created MouseMoveEvent and enqueue it
            if (distance >= Threshold)
            {
                var @event = new MouseMoveEvent { MousePosition = currentMousePositionAsPoint, IssuingModule = MouseModule.Identifier};
                Enqueue(@event);
            }
        }

        /// <summary>
        ///     start the mouse movement capture by starting the timer that records mouse position.
        /// </summary>
        public void StartCapture()
        {
            NativeMethods.GetCursorPos(out lastMousePosition);

            var samplingTimeIntervalInMilliseconds = (int) ((double) 1 / SamplingRateInHz * 1000);

            mousePositionRecordingTimer = new Timer(GetMousePosition, null, 0, samplingTimeIntervalInMilliseconds);
        }

        /// <summary>
        ///     stop the mouse movement capture by disposing the timer that records mouse position.
        /// </summary>
        public void StopCapture()
        {
            mousePositionRecordingTimer?.Dispose();
            Close();
        }
    }
}