using System.Text.Json;

namespace MORR.Modules.WebBrowser
{
    internal interface IWebBrowserEventObserver
    {
        public EventLabel HandledEventLabel { get; }
        public void Notify(JsonElement eventJson);
    }

    internal interface IWebBrowserEventObservable
    {
        public void Subscribe(IWebBrowserEventObserver observer, params EventLabel[] eventLabel);
        public void Unsubscribe(IWebBrowserEventObserver observer, params EventLabel[] eventLabel);
    }
}