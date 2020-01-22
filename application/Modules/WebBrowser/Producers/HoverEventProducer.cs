using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for HoverEvent
    /// </summary>
    [Export(typeof(HoverEventProducer))]
    [Export(typeof(EventQueue<HoverEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class HoverEventProducer : BoundedMultiConsumerEventQueue<HoverEvent>
    {
        // TODO: Implement this
    }
}