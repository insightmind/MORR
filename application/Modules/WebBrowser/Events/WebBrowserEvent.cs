using System;
using MORR.Shared;

namespace MORR.Modules.WebBrowser.Events
{
    /// <summary>
    /// A generic web browser event which all specific WebBrowserEvents inherit from.
    /// </summary>
    public abstract class WebBrowserEvent : Event
    {
        /// <summary>
        ///     The identifier of the tab where the web browser event occured in
        /// </summary>
        public Guid TabID { get; set; }

        /// <summary>
        ///     The URL of the website where the web browser event occured in
        /// </summary>
        public Uri CurrentURL { get; set; }
    }

}
