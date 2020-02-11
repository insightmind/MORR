using System.Text.Json;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for NavigationEvent
    /// </summary>
    public class NavigationEventProducer : WebBrowserEventProducer<NavigationEvent>
    {
        public override EventLabel HandledEventLabel => EventLabel.NAVIGATION;
    }
}