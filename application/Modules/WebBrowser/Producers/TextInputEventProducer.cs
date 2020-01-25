using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
﻿using System.ComponentModel.Composition;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for TextInputEvent
    /// </summary>
    [Export(typeof(TextInputEventProducer))]

    [Export(typeof(IReadOnlyEventQueue<TextInputEvent>))]
    [Export(typeof(WebBrowserEventProducer<TextInputEvent>))]
    [Export(typeof(WebBrowserEventProducer<>))]
    public class TextInputEventProducer : WebBrowserEventProducer<TextInputEvent>
    {

    }
}