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

        public override string Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override void Deserialize(string serialized)
        {
            throw new System.NotImplementedException();
        }
    }
}
