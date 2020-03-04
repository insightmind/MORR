using System;
using System.Windows.Input;

namespace MORR.Modules.Keyboard.Events
{
    /// <summary>
    ///     A keyboard user interaction
    /// </summary>
    public class KeyboardInteractEvent : KeyboardEvent
    {
        /// <summary>
        ///     The key that was pressed
        /// </summary>
        public Key PressedKey { get; set; }

        /// <summary>
        ///     The modifier keys to the key pressed
        /// </summary>
        public ModifierKeys ModifierKeys { get; set; }

        /// <summary>
        ///     The actual user input according to Input locale
        /// </summary>
        public char MappedCharacter { get; set; }
    }
}