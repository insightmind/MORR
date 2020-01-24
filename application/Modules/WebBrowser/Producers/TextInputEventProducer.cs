using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for TextInputEvent
    /// </summary>
    [Export(typeof(TextInputEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<TextInputEvent>))]
    public class TextInputEventProducer : DefaultEventQueue<TextInputEvent>
    {
        // TODO: Implement this
    }
}