using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

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
        public int NewTabID { get; set; }

        protected override void DeserializeSpecificAttributes(JsonElement parsed)
        {
            NewTabID = parsed.GetProperty("newTabID").GetInt32();
        }
    }
}
