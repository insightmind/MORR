using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;


namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for TextSelectionEvent
    /// </summary>
    [Export(typeof(TextSelectionEventProducer))]

    [Export(typeof(IReadOnlyEventQueue<TextSelectionEvent>))]
    [Export(typeof(WebBrowserEventProducer<TextSelectionEvent>))]
    [Export(typeof(WebBrowserEventProducer<>))]
    public class TextSelectionEventProducer : WebBrowserEventProducer<TextSelectionEvent>
    {

    }
}