using System.Text.Json;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for HoverEvent
    /// </summary>
    public class HoverEventProducer : WebBrowserEventProducer<HoverEvent>
    {
        public override void Notify(JsonElement eventJson)
        {
            var @event = new HoverEvent();
            @event.Deserialize(eventJson);
            Enqueue(@event);
        }

        public override EventLabel HandledEventLabel => EventLabel.HOVER;
    }
}