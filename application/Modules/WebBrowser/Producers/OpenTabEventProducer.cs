using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for OpenTabEvent
    /// </summary>
    [Export(typeof(OpenTabEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<OpenTabEvent>))]
    public class OpenTabEventProducer : DefaultEventQueue<OpenTabEvent>
    {
        // TODO: Implement this
    }
}