namespace MORR.Modules.Clipboard.Events
{
    /// <summary>
    ///     A clipboard user interaction
    /// </summary>
    public class ClipboardInteractEvent : ClipboardEvent
    {
        /// <summary>
        ///     An Enum to specify the four interaction type with a clip board:
        ///     clear, copy, cut and paste
        /// </summary>
        public enum InteractionType
        {
            Clear, Copy, Cut, Paste
        };

        /// <summary>
        ///     The interaction type
        /// </summary>
        public InteractionType Interaction { get; set;}

        /// <summary>
        ///     The text in the clipboard
        /// </summary>
        public string Text { get; set; }
    }
}
