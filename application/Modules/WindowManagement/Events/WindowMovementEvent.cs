using System.Numerics;
using System.Windows;

namespace MORR.Modules.WindowManagement.Events
{
    /// <summary>
    ///     A window movement user interaction
    /// </summary>
    public class WindowMovementEvent : WindowEvent
    {
        /// <summary>
        ///     The old location of the window
        /// </summary>
        public Point OldLocation { get; set; }

        /// <summary>
        ///     The new location of the window
        /// </summary>
        public Point NewLocation { get; set; }
    }
}