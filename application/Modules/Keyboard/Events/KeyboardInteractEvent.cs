using System.Windows.Input;

namespace MORR.Modules.Keyboard.Events
{
    /// <summary>
    ///     A keyboard user interaction
    /// </summary>
    public class KeyboardInteractEvent : KeyboardEvent
    {
        /// <summary>
        ///     The key that was pressed.
        ///     This field is for data processing.
        /// </summary>
        public Key PressedKey_System_Windows_Input_Key { get; set; }

        /// <summary>
        ///     The name of the key on the keyboard that is pressed
        ///     This field is only for increasing the human readability. 
        /// </summary>
        public string PressedKeyName { get; set; } = "";

        /// <summary>
        ///     The modifier keys to the key pressed.
        ///     This field is for data processing
        /// </summary>
        public ModifierKeys ModifierKeys_System_Windows_Input_ModifierKeys { get; set; }

        /// <summary>
        ///     The name of the key on the keyboard that is pressed
        ///     This field is only for increasing the human readability. 
        /// </summary>
        public string ModifierKeysName { get; set; } = "";

        /// <summary>
        ///     The actual user input according to Input locale.
        ///     If the event has '\u0000' at this property, the
        ///     pressed key is not mapped to a unicode. It can
        ///     be Shift, Alt etc.
        ///     This field is only for increasing the human readability.
        /// </summary>
        public char MappedCharacter_Unicode { get; set; }
    }
}