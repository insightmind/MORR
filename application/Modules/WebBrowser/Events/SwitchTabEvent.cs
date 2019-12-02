using System;
using System.Collections.Generic;
using System.Text;

namespace MORR.Modules.WebBrowser.Events
{
    /// <summary>
    /// A switch tab user interaction
    /// </summary>
    public class SwitchTabEvent : WebBrowserEvent
    {
        /// <summary>
        /// The identifier of the tab that the user switched to
        /// </summary>
        public Guid NewTabID { get; set; }
    }
}
