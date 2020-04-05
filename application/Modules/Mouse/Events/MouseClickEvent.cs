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
        /// </summary>
        public MouseAction MouseAction { get; set; }

        /// <summary>
        ///     The handle of the window in which the mouse click occurred in String.
        /// </summary>
        public string HWnd { get; set; } = "";
    }
}