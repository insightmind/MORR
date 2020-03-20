using System.Text.Json;

namespace MORR.Modules.WebBrowser.Events
{
    /// <summary>
    ///     A text input user interaction
    /// </summary>
    public class TextInputEvent : WebBrowserEvent
    {
        private const string serializedTextField = "text";
        private const string serializedTextBoxField = "target";

        /// <summary>
        ///     The text that was inputted by the user on the website
        /// </summary>
        public string InputtedText { get; private set; } = "";

        /// <summary>
        ///     The textbox where the text was inputted in
        /// </summary>
        public string Textbox { get; private set; } = "";

        protected override void DeserializeSpecificAttributes(JsonElement parsed)
        {
            InputtedText = parsed.GetProperty(serializedTextField).GetString();
            Textbox = parsed.GetProperty(serializedTextBoxField).GetString();
        }
    }
}