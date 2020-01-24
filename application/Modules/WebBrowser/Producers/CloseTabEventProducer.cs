using System;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for CloseTabEvent
    /// </summary>
    [Export(typeof(CloseTabEventProducer))]
    [Export(typeof(EventQueue<CloseTabEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(WebBrowserEventProducer<CloseTabEvent>))]
    [Export(typeof(WebBrowserEventProducer<WebBrowserEvent>))]
    public class CloseTabEventProducer :  WebBrowserEventProducer<CloseTabEvent>
    {
    }
}