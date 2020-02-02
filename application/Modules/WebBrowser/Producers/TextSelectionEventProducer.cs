using System.Text.Json;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for TextSelectionEvent
    /// </summary>
    public class TextSelectionEventProducer : WebBrowserEventProducer<TextSelectionEvent>
    {
        public override EventLabel HandledEventLabel => EventLabel.TEXTSELECTION;

        public override void Notify(JsonElement eventJson)
        {
            var @event = new TextSelectionEvent();
            @event.Deserialize(eventJson);
            Enqueue(@event);
        }
    }
}