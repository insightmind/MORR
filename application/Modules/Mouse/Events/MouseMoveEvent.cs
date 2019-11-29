using System.Numerics;
using MORR.Shared;

namespace MORR.Modules.Mouse.Events
{
    /// <summary>
    ///     A mouse move user interaction
    /// </summary>
    public class MouseMoveEvent : Event
    {
        /// <summary>
        ///     The movement of the mouse
        /// </summary>
        public Vector2 Movement { get; set; }
    }
}