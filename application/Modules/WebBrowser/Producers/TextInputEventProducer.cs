using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using System.ComponentModel.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for TextInputEvent
    /// </summary>
    [Export(typeof(TextInputEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<TextInputEvent>))]
    public class TextInputEventProducer : BoundedMultiConsumerEventQueue<TextInputEvent>
    {
        // TODO: Implement this
    }
}