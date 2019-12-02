using System;
using System.Collections.Generic;
using System.Text;

namespace MORR.Modules.WebBrowser.Events
{
    /// <summary>
    /// A text selection user interaction
    /// </summary>
    public class TextSelectionEvent : WebBrowserEvent
    {
        /// <summary>
        /// The text that was selected on the website
        /// </summary>
        public string SelectedText { get; set; }
    }
}
