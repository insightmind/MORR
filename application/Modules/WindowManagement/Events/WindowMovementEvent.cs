using System.Numerics;

namespace MORR.Modules.Mouse.Events
{
    /// <summary>
    ///     A window movement user interaction
    /// </summary>
    public class WindowMovementEvent : WindowEvent
    {
        /// <summary>
        ///     The old location of the window
        /// </summary>
        public Vector2 OldLocation { get; set; }

        /// <summary>
        ///     The new location of the window
        /// </summary>
        public Vector2 NewLocation { get; set; }
    }
}