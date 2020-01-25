using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MORR.Modules.WebBrowser.Events
{
    /// <summary>
    /// A button click user interaction
    /// </summary>
    public class ButtonClickEvent : WebBrowserEvent
    {
        /// <summary>
        /// The title of the button item that was clicked on the website
        /// </summary>
        public string Button { get; set; }

        /// <summary>
        /// The URL of the website that the button is linked to
        /// </summary>
        public string? Href { get; set; }

        protected override void DeserializeSpecificAttributes(JsonElement parsed)
        {
            Button = parsed.GetProperty("buttonTitle").GetString();
            if (parsed.TryGetProperty("buttonHref", out var hrefElement))
                Href = hrefElement.GetString();
        }
    }
}
