using System.ComponentModel.Composition;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for SwitchTabEvent
    /// </summary>
    [Export(typeof(SwitchTabEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<SwitchTabEvent>))]
    public class SwitchTabEventProducer : BoundedMultiConsumerEventQueue<SwitchTabEvent>
    {
        // TODO: Implement this
    }
}
