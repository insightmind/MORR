using System.Text.Json;

namespace MORR.Modules.WebBrowser.Events
{
    /// <summary>
    ///     A button click user interaction
    /// </summary>
    public class ButtonClickEvent : WebBrowserEvent
    {
        private const string serializedButtonField = "buttonTitle";
        private const string serializedHrefField = "buttonHref";

        /// <summary>
        ///     The title of the button item that was clicked on the website
        /// </summary>
        public string Button { get; set; }

        /// <summary>
        ///     The URL of the website that the button is linked to
        /// </summary>
        public string? Href { get; set; }

        protected override void DeserializeSpecificAttributes(JsonElement parsed)
        {
            Button = parsed.GetProperty(serializedButtonField).GetString();
            if (parsed.TryGetProperty(serializedHrefField, out var hrefElement))
            {
                Href = hrefElement.GetString();
            }
        }
    }
}