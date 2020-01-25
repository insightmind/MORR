using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
﻿using System.ComponentModel.Composition;


namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for OpenTabEvent
    /// </summary>

    [Export(typeof (OpenTabEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<OpenTabEvent>))]
    [Export(typeof(WebBrowserEventProducer<OpenTabEvent>))]
    [Export(typeof(IWebBrowserEventObserver))]
    public class OpenTabEventProducer : WebBrowserEventProducer<OpenTabEvent>
    {
    
    }
}