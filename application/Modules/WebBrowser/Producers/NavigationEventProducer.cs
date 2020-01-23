﻿using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for NavigationEvent
    /// </summary>
    [Export(typeof(NavigationEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<NavigationEvent>))]
    [Shared]
    public class NavigationEventProducer : BoundedMultiConsumerEventQueue<NavigationEvent>
    {
        // TODO: Implement this
    }
}