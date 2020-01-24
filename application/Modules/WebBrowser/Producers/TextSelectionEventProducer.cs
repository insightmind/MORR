using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using System.ComponentModel.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for TextSelectionEvent
    /// </summary>
    [Export(typeof(TextSelectionEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<TextSelectionEvent>))]
    public class TextSelectionEventProducer : BoundedMultiConsumerEventQueue<TextSelectionEvent>
    {
        // TODO: Implement this
    }
}