using MORR.Shared.Events;

namespace MORR.Modules.Keyboard.Events
{
    /// <summary>
    ///     A generic keyboard event which all specific KeyboardEvents inherit from.
    ///     This class is empty because it is used only for restricting type usage.
    /// </summary>
    public abstract class KeyboardEvent : Event { }
}