using System;
using System.Collections.Generic;
using System.Composition;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for SwitchTabEvent
    /// </summary>
    [Export(typeof(SwitchTabEventProducer))]
    [Export(typeof(EventQueue<SwitchTabEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(WebBrowserEventProducer<SwitchTabEvent>))]
    [Export(typeof(WebBrowserEventProducer<WebBrowserEvent>))]
    public class SwitchTabEventProducer : WebBrowserEventProducer<SwitchTabEvent>
    {

    }
}
