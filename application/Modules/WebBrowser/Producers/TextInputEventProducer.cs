using System.Text.Json;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for TextInputEvent
    /// </summary>
    public class TextInputEventProducer : WebBrowserEventProducer<TextInputEvent>
    {
        public override EventLabel HandledEventLabel => EventLabel.TEXTINPUT;
    }
}