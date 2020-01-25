using System.ComponentModel.Composition;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseScrollEvent
    /// </summary>
    [Export(typeof(MouseScrollEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<MouseScrollEvent>))]
    public class MouseScrollEventProducer : DefaultEventQueue<MouseScrollEvent>
    {
        // TODO: Implement this
    }
}