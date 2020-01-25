﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Web;

namespace MORR.Modules.WebBrowser.Events
{
    /// <summary>
    /// A hover user interaction
    /// </summary>
    public class HoverEvent : WebBrowserEvent
    {
        /// <summary>
        /// The element on the website that has been hovered
        /// </summary>
        public string HoveredElement { get; set; }

        protected override void DeserializeSpecificAttributes(JsonElement parsed)
        {
            HoveredElement = parsed.GetProperty("target").GetString();
        }
    }
}
