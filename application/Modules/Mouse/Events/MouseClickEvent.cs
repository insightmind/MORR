using System;
using System.Windows.Input;

namespace MORR.Modules.Mouse.Events
{
    /// <summary>
    ///     A mouse click user interaction
    /// </summary>
    public class MouseClickEvent : MouseEvent
    {
        /// <summary>
        ///     The button that was clicked
        /// </summary>
        public MouseButton Button { get; set; }

        /// <summary>
        ///     The state of the button that was clicked
        /// </summary>
        public MouseButtonState State { get; set; }

        /// <summary>
        ///     The handle of the window in which the mouse click occurred
        /// </summary>
        public IntPtr HWnd { get; set; }
    }
}