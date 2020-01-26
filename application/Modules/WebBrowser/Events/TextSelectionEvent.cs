﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MORR.Modules.WebBrowser.Events
{
    /// <summary>
    /// A text selection user interaction
    /// </summary>
    public class TextSelectionEvent : WebBrowserEvent
    {
        private static readonly string serializedTextField = "textSelection";
        /// <summary>
        /// The text that was selected on the website
        /// </summary>
        public string SelectedText { get; set; }

        protected override void DeserializeSpecificAttributes(JsonElement parsed)
        {
            SelectedText = parsed.GetProperty(serializedTextField).GetString();
        }
    }
}
