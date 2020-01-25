using System;

namespace MORR.Modules.WebBrowser
{
    /// <summary>
    ///     Enum used to map the incoming serialized data (from the browser extension) to the event classes.
    /// </summary>
    internal enum EventLabel
    {
        BUTTONCLICK,
        CLOSETAB,
        DOWNLOAD,
        HOVER,
        NAVIGATION,
        OPENTAB,
        SWITCHTAB,
        TEXTINPUT,
        TEXTSELECTION
    }
}