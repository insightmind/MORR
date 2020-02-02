using System.Text.Json;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for NavigationEvent
    /// </summary>
    public class NavigationEventProducer : WebBrowserEventProducer<NavigationEvent>
    {
        public override void Notify(JsonElement eventJson)
        {
            var @event = new NavigationEvent();
            @event.Deserialize(eventJson);
            Enqueue(@event);
        }

        public override EventLabel HandledEventLabel => EventLabel.NAVIGATION;
    }
}