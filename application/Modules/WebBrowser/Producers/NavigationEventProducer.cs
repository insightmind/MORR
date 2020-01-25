using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using System.ComponentModel.Composition;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for NavigationEvent
    /// </summary>
    [Export(typeof(NavigationEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<NavigationEvent>))]
    [Export(typeof(WebBrowserEventProducer<NavigationEvent>))]
    [Export(typeof(WebBrowserEventProducer<>))]
    public class NavigationEventProducer :  WebBrowserEventProducer<NavigationEvent>
    {

    }
}