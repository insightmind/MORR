using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for TextSelectionEvent
    /// </summary>
    [Export(typeof(TextSelectionEventProducer))]
    [Export(typeof(EventQueue<TextSelectionEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(WebBrowserEventProducer<TextSelectionEvent>))]
    [Export(typeof(WebBrowserEventProducer<WebBrowserEvent>))]
    public class TextSelectionEventProducer : WebBrowserEventProducer<TextSelectionEvent>
    {
    }
}