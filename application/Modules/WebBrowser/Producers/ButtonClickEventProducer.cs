using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ButtonClickEvent
    /// </summary>
    [Export(typeof(ButtonClickEventProducer))]
    [Export(typeof(EventQueue<ButtonClickEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class ButtonClickEventProducer : EventQueue<ButtonClickEvent>
    {
        /// <summary>
        ///     Asynchronously gets all button click events as ButtonClickEvent type
        /// </summary>
        /// <returns>A stream of ButtonClickEvent</returns>
        public override IAsyncEnumerable<ButtonClickEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asynchronously enqueues a new button click event
        /// </summary>
        /// <param name="event">The button click event to enqueue</param>
        protected override void Enqueue(ButtonClickEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}