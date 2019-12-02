using System;

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
    }
}
 