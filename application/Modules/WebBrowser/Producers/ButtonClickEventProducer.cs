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

        public override void Notify(JsonElement eventJson)
        {
            var @event = new ButtonClickEvent();
            @event.Deserialize(eventJson);
            Enqueue(@event);
        }
    }
}