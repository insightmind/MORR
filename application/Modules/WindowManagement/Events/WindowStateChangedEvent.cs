using System.Windows;

namespace MORR.Modules.Mouse.Events
{
    /// <summary>
    ///     An user interaction that changes the state of a window
    /// </summary>
    public class WindowStateChangedEvent: WindowEvent
    {
        /// <summary>
        /// The new State of the window
        /// </summary>
        public WindowState State;
    }
}
