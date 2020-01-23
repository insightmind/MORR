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
    ///     Provides a single-writer-multiple-reader queue for ButtonClickEvent
    /// </summary>
    [Export(typeof(ButtonClickEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<ButtonClickEvent>))]
    [Shared]
    public class ButtonClickEventProducer : BoundedMultiConsumerEventQueue<ButtonClickEvent>
    {
        // TODO: Implement this
    }
}