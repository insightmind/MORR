using System;
using System.Text.Json;

namespace MORR.Modules.WebBrowser.Events
{
    /// <summary>
    /// A text input user interaction
    /// </summary>
    public class TextInputEvent : WebBrowserEvent
    {
        /// <summary>
        /// The text that was inputted by the user on the website
        /// </summary>
       public string InputtedText { get; set; }

        /// <summary>
        /// The textbox where the text was inputted in
        /// </summary>
       public string Textbox { get; set; }

        protected override void DeserializeSpecificAttributes(JsonDocument parsed)
        {
            var root = parsed.RootElement;
            InputtedText = root.GetProperty("text").GetString();
            Textbox = root.GetProperty("target").GetString();
        }
    }
}
 