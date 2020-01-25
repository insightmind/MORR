using System;

namespace MORR.Modules.Mouse.Events
{
    /// <summary>
    ///     A mouse scroll user interaction
    /// </summary>
    public class MouseScrollEvent : MouseEvent
    {
        /// <summary>
        ///     The amount of the wheel being scrolled.
        ///     relative standard: One wheel click is defined as WHEEL_DELTA, which is 120.
        /// </summary>
        public short ScrollAmount { get; set; }

        /// <summary>
        ///     The handle of the window in which the mouse scroll occurred
        /// </summary>
        public IntPtr HWnd { get; set; }
    }
}