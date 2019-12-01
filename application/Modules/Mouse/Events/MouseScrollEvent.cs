using System;

namespace MORR.Modules.Mouse.Events
{
    /// <summary>
    ///     A mouse scroll user interaction
    /// </summary>
    public class MouseScrollEvent : MouseEvent
    {
        /// <summary>
        ///     The amount of the wheel being scrolled
        /// </summary>
        public short ScrollAmount { get; set; }

        /// <summary>
        ///     The handle of the window in which the mouse scroll occurred
        /// </summary>
        public IntPtr HWnd { get; set; }
    }
}