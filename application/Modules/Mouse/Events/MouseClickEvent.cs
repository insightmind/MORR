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
        ///     Specifies constants that define actions performed by the mouse.
        ///     0-None
        ///     1-LeftClick
        ///     2-RightClick
        ///     3-MiddleClick
        ///     5-LeftDoubleClick
        ///     6-RightDoubleClick
        ///     7-MiddleDoubleClick
        /// </summary>
        public MouseAction MouseAction { get; set; }

        /// <summary>
        ///     The handle of the window in which the mouse click occurred in String.
        /// </summary>
        public string HWnd { get; set; }
    }
}