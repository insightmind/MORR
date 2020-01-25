using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using System.ComponentModel.Composition;
namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ButtonClickEvent
    /// </summary>
    [Export(typeof(ButtonClickEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<ButtonClickEvent>))]
    [Export(typeof(WebBrowserEventProducer<ButtonClickEvent>))]
    [Export(typeof(WebBrowserEventProducer<>))]
    public class ButtonClickEventProducer :  WebBrowserEventProducer<ButtonClickEvent>
    {

    }
}