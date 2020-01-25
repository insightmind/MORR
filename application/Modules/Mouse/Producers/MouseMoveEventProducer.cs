using System.ComponentModel.Composition;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseMoveEvent
    /// </summary>
    [Export(typeof(MouseMoveEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<MouseMoveEvent>))]
    public class MouseMoveEventProducer : DefaultEventQueue<MouseMoveEvent>
    {
        // TODO: Implement this
    }
}