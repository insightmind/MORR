using MORR.Shared.Events;

namespace MORR.Modules.Clipboard.Events
{
    /// <summary>
    ///     A generic clipboard event which all specific ClipboardEvents inherit from.
    /// </summary>
    public abstract class ClipboardEvent : Event
    {
        /// <summary>
        ///     The text in the clipboard
        /// </summary>
        public string Text { get; set; }
    }
}