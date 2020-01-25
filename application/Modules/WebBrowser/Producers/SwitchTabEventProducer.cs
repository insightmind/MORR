using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
﻿using System.ComponentModel.Composition;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for SwitchTabEvent
    /// </summary>
    [Export(typeof(SwitchTabEventProducer))]

    [Export(typeof(IReadOnlyEventQueue<SwitchTabEvent>))]
    [Export(typeof(WebBrowserEventProducer<SwitchTabEvent>))]
    [Export(typeof(WebBrowserEventProducer<>))]
    public class SwitchTabEventProducer : WebBrowserEventProducer<SwitchTabEvent>
    {
    

    }
}