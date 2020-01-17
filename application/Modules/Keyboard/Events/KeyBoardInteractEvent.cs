using System.Windows.Forms;

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
        public Keys PressedKey { get; set; }

        public KeyboardInteractEvent(Keys key)
        {
            this.PressedKey = key;
        }

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
