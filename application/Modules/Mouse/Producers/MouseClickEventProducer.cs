using System.ComponentModel.Composition;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseClickEvent
    /// </summary>
    [Export(typeof(MouseClickEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<MouseClickEvent>))]
    public class MouseClickEventProducer : DefaultEventQueue<MouseClickEvent>
    {
        // TODO: Implement this
    }
}