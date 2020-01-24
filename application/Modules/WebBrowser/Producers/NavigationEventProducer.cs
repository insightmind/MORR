using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for NavigationEvent
    /// </summary>
    [Export(typeof(NavigationEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<NavigationEvent>))]
    public class NavigationEventProducer : DefaultEventQueue<NavigationEvent>
    {
        // TODO: Implement this
    }
}