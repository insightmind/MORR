using System.Text.Json;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for SwitchTabEvent
    /// </summary>
    public class SwitchTabEventProducer : WebBrowserEventProducer<SwitchTabEvent>
    {
        public override EventLabel HandledEventLabel => EventLabel.SWITCHTAB;
    }
}