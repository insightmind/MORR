using System.Windows.Input;

namespace MORR.Modules.Keyboard.Events
{
    /// <summary>
    ///     A keyboard user interaction
    /// </summary>
    public class KeyBoardInteractEvent : KeyboardEvent
    {
        /// <summary>
        ///     The key that was pressed
        /// </summary>
        public Key PressedKey { get; set; }
    }
}
