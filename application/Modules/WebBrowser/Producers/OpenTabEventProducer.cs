using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for OpenTabEvent
    /// </summary>
    [Export(typeof (OpenTabEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<OpenTabEvent>))]
    [Shared]
    public class OpenTabEventProducer : BoundedMultiConsumerEventQueue<OpenTabEvent>
    {
        // TODO: Implement this
    }
}
