using System;
using System.Collections.Generic;
using System.Text;
using System.Composition;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for SwitchTabEvent
    /// </summary>
    [Export(typeof(SwitchTabEventProducer))]
    [Export(typeof(EventQueue<SwitchTabEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class SwitchTabEventProducer : EventQueue<SwitchTabEvent>
    {
        /// <summary>
        ///     Asynchronously gets all switch tab events as SwitchTabEvent type
        /// </summary>
        /// <returns>A stream of SwitchTabEvent</returns>
        public override IAsyncEnumerable<SwitchTabEvent> GetEvents()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///     Asynchronously enqueues a new switch tab event
        /// </summary>
        /// <param name="event">The switch tab event to enqueue</param>
        protected override void Enqueue(SwitchTabEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
