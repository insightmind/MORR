using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
