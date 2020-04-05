using System.Text.Json;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for CloseTabEvent
    /// </summary>
    public class CloseTabEventProducer : WebBrowserEventProducer<CloseTabEvent>
    {
        public override EventLabel HandledEventLabel => EventLabel.CLOSETAB;
    }
}