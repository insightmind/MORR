using System.Text.Json;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for OpenTabEvent
    /// </summary>
    public class OpenTabEventProducer : WebBrowserEventProducer<OpenTabEvent>
    {
        public override void Notify(JsonElement eventJson)
        {
            var @event = new OpenTabEvent();
            @event.Deserialize(eventJson);
            Enqueue(@event);
        }

        public override EventLabel HandledEventLabel => EventLabel.OPENTAB;
    }
}