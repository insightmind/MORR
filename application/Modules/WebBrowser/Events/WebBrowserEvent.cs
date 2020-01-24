using System;
using System.Text.Json;
using MORR.Shared.Events;

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

        /// <summary>
        ///     Deserialize a browser event from a string.
        /// </summary>
        /// <param name="serialized">The serialized event</param>
        public void Deserialize(string serialized)
        {
            this.Deserialize(JsonDocument.Parse(serialized));
        }

        /// <summary>
        ///     Deserialize a browser event from a JsonDocument instance
        /// </summary>
        /// <param name="serialized">A JsonDocument parsed from a serialized event.</param>
        public void Deserialize(JsonDocument parsed)
        {
            DeserializeCommonAttributes(parsed);
            DeserializeSpecificAttributes(parsed);
        }

        /// <summary>
        ///     Deserialize the attributes specific to the event type.
        /// </summary>
        /// <param name="parsed"></param>
        protected virtual void DeserializeSpecificAttributes(JsonDocument parsed)
        {

        }

        /// <summary>
        ///     Deserialize the attributes shared by all browser event types.
        /// </summary>
        /// <param name="parsed"></param>
        protected void DeserializeCommonAttributes(JsonDocument parsed)
        {
            var root = parsed.RootElement;
            TabID = root.GetProperty("tabID").GetGuid();
            CurrentURL = new Uri(root.GetProperty("url").ToString());
            Timestamp = root.GetProperty("timeStamp").GetDateTime();
            IssuingModule = new Guid("e9240dc4-f33f-43db-a419-5b36d8279c88"); //TODO: actually obtain this from the module
        }
    }

}
