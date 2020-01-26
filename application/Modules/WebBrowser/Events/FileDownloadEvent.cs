using System;
using System.Text.Json;

namespace MORR.Modules.WebBrowser.Events
{
    /// <summary>
    /// A file download user interaction
    /// </summary>
    public class FileDownloadEvent : WebBrowserEvent
    {
        private static readonly string serializedFileURLField = "fileURL";
        private static readonly string serializedmimeTypeField = "mimeType";
        /// <summary>
        /// The URL of the file that was downloaded
        /// </summary>
        public Uri FileURL { get; set; }

        /// <summary>
        /// MIME type of the file that was downloaded
        /// </summary>
        public string MIMEType { get; set; }

        protected override void DeserializeSpecificAttributes(JsonElement parsed)
        {
            FileURL = new Uri(parsed.GetProperty(serializedFileURLField).GetString());
            MIMEType = parsed.GetProperty(serializedmimeTypeField).GetString();
        }
    }
}
