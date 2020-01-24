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
        /// </summary>
        public MouseAction MouseAction { get; set; }

        /// <summary>
        ///     The handle of the window in which the mouse click occurred.
        /// </summary>
        public IntPtr HWnd { get; set; }

        public override string Serialize()
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(string serialized)
        {
            throw new NotImplementedException();
        }
    }
}