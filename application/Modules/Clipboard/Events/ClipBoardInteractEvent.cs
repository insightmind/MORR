using System;

namespace MORR.Modules.Clipboard.Events
{
    /// <summary>
    ///     A clipboard user interaction
    /// </summary>
    public class ClipBoardInteractEvent : ClipboardEvent
    {
        /// <summary>
        ///     An Enum to specify the four interaction type with a clip board:
        ///     clear, copy, cut and paste
        /// </summary>
        public enum InteractionType
        {
            CLEAR, COPY, CUT, PASTE
        };

        /// <summary>
        ///     The interaction type
        /// </summary>
        public InteractionType IAType { get; set;}

        /// <summary>
        ///     The text in the clipboard
        /// </summary>
        public string Text { get; set; }
    }
}
