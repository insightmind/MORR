using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
﻿using System.ComponentModel.Composition;



namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for CloseTabEvent
    /// </summary>
    [Export(typeof(CloseTabEventProducer))]
    [Export(typeof(WebBrowserEventProducer<CloseTabEvent>))]
    [Export(typeof(WebBrowserEventProducer<WebBrowserEvent>))]
    [Export(typeof(IReadOnlyEventQueue<CloseTabEvent>))]
    public class CloseTabEventProducer :  WebBrowserEventProducer<CloseTabEvent>
    {

    }
}