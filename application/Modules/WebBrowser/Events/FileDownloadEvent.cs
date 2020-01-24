using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MORR.Modules.WebBrowser.Events
{
    /// <summary>
    /// A file download user interaction
    /// </summary>
    public class FileDownloadEvent : WebBrowserEvent
    {
        /// <summary>
        /// The URL of the file that was downloaded
        /// </summary>
        public Uri FileURL { get; set; }

        /// <summary>
        /// MIME type of the file that was downloaded
        /// </summary>
        public string MIMEType { get; set; }

        protected override void DeserializeSpecificAttributes(JsonDocument parsed)
        {
            var root = parsed.RootElement;
            FileURL = new Uri(root.GetProperty("fileURL").GetString());
            MIMEType = root.GetProperty("mimeType").GetString();
        }
    }
}
