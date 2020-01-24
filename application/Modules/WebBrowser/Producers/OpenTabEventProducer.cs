using System;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for OpenTabEvent
    /// </summary>
    [Export(typeof (OpenTabEventProducer))]
    [Export(typeof(EventQueue<OpenTabEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(WebBrowserEventProducer<OpenTabEvent>))]
    [Export(typeof(WebBrowserEventProducer<WebBrowserEvent>))]
    public class OpenTabEventProducer : WebBrowserEventProducer<OpenTabEvent>
    {

    }
}
