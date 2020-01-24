using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ButtonClickEvent
    /// </summary>
    [Export(typeof(ButtonClickEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<ButtonClickEvent>))]
    public class ButtonClickEventProducer : BoundedMultiConsumerEventQueue<ButtonClickEvent>
    {
        // TODO: Implement this
    }
}