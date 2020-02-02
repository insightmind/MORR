namespace MORR.Modules.Mouse.Events
{
    /// <summary>
    ///     A mouse scroll user interaction
    /// </summary>
    public class MouseScrollEvent : MouseEvent
    {
        /// <summary>
        ///     The amount of the wheel being scrolled.
        ///     A positive value indicates that the wheel was rotated forward,
        ///     away from the user; a negative value indicates that the wheel
        ///     was rotated backward, toward the user.
        ///     One wheel click is defined as WHEEL_DELTA, which is 120.
        /// </summary>
        public short ScrollAmount { get; set; }

        /// <summary>
        ///     The handle of the window in which the mouse scroll occurred in String.
        /// </summary>
        public string HWnd { get; set; }
    }
}