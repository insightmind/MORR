using System;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for TextInputEvent
    /// </summary>
    [Export(typeof(TextInputEventProducer))]
    [Export(typeof(EventQueue<TextInputEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(WebBrowserEventProducer<TextInputEvent>))]
    [Export(typeof(WebBrowserEventProducer<WebBrowserEvent>))]
    public class TextInputEventProducer : WebBrowserEventProducer<TextInputEvent>
    {

    }
}