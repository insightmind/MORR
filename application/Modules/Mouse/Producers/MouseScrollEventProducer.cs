using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseScrollEvent
    /// </summary>
    [Export(typeof(MouseScrollEventProducer))]
    [Export(typeof(EventQueue<MouseScrollEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class MouseScrollEventProducer : BoundedMultiConsumerEventQueue<MouseScrollEvent>
    {
        // TODO: Implement this
    }
}