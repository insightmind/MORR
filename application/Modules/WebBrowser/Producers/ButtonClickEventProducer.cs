using System.Text.Json;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ButtonClickEvent
    /// </summary>
    public class ButtonClickEventProducer : WebBrowserEventProducer<ButtonClickEvent>
    {
        public override EventLabel HandledEventLabel => EventLabel.BUTTONCLICK;
    }
}