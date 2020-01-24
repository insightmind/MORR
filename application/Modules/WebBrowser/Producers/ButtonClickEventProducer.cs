using System;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ButtonClickEvent
    /// </summary>
    [Export(typeof(ButtonClickEventProducer))]
    [Export(typeof(EventQueue<ButtonClickEvent>))]
    [Export(typeof(EventQueue<Event>))]
    [Export(typeof(WebBrowserEventProducer<ButtonClickEvent>))]
    [Export(typeof(WebBrowserEventProducer<WebBrowserEvent>))]
    public class ButtonClickEventProducer :  WebBrowserEventProducer<ButtonClickEvent>
    {

    }
}