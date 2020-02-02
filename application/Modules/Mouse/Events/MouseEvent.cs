using MORR.Shared.Events;
using System.Windows;

namespace MORR.Modules.Mouse.Events
{
    /// <summary>
    ///     A generic mouse event which all specific MouseEvents inherit from.
    /// </summary>
    public abstract class MouseEvent : Event
    {
        /// <summary>
        ///     The current position of the mouse in screen coordinates
        /// </summary>
        public Point MousePosition { get; set; }
    }
}