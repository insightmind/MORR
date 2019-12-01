using System.Drawing;

namespace MORR.Modules.Mouse.Events
{
    /// <summary>
    ///     A window resizing user interaction
    /// </summary>
    public class WindowResizingEvent : WindowEvent
    {
        /// <summary>
        ///     The old size of the window
        /// </summary>
        public Size OldSize { get; set; }

        /// <summary>
        ///     The new size of the window
        /// </summary>
        public Size NewSize { get; set; }
    }
}