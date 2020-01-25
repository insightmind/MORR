using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
﻿using System.ComponentModel.Composition;
using MORR.Shared.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for CloseTabEvent
    /// </summary>
    [Export(typeof(CloseTabEventProducer))]
    [Export(typeof(WebBrowserEventProducer<CloseTabEvent>))]
    [Export(typeof(IReadOnlyEventQueue<CloseTabEvent>))]
    [Export(typeof(IWebBrowserEventObserver))]
    [Export(typeof(IReadOnlyEventQueue<Event>))]
    [Export(typeof(IReadWriteEventQueue<CloseTabEvent>))]
    public class CloseTabEventProducer :  WebBrowserEventProducer<CloseTabEvent>
    {

    }
}