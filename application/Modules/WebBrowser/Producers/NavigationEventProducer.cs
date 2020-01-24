using System;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for NavigationEvent
    /// </summary>
    [Export(typeof(NavigationEventProducer))]
    [Export(typeof(EventQueue<NavigationEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(WebBrowserEventProducer<NavigationEvent>))]
    [Export(typeof(WebBrowserEventProducer<WebBrowserEvent>))]
    public class NavigationEventProducer :  WebBrowserEventProducer<NavigationEvent>
    {
    }
}