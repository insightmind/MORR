using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WindowManagement.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowFocusEvent
    /// </summary>
    [Export(typeof(WindowFocusEventProducer))]
    [Export(typeof(EventQueue<WindowFocusEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class WindowFocusEventProducer : BoundedMultiConsumerEventQueue<WindowFocusEvent>
    {
        // TODO: Implement this
    }
}