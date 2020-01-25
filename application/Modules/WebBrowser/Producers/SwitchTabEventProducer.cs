using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for SwitchTabEvent
    /// </summary>
    [Export(typeof(SwitchTabEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<SwitchTabEvent>))]
    public class SwitchTabEventProducer : DefaultEventQueue<SwitchTabEvent>
    {
        // TODO: Implement this
    }
}