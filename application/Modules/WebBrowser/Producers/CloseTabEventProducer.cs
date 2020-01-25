using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for CloseTabEvent
    /// </summary>
    [Export(typeof(CloseTabEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<CloseTabEvent>))]
    public class CloseTabEventProducer : DefaultEventQueue<CloseTabEvent>
    {
        // TODO: Implement this
    }
}