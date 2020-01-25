using System;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser
{
    internal interface IWebBrowserEventObserver
    {
        public Type HandledEventType { get; }
        public void Notify(WebBrowserEvent @event);
    }

    internal interface IWebBrowserEventObservible
    {
        public void SubScribe(IWebBrowserEventObserver observer, Type eventType);
        public void UnSubScribe(IWebBrowserEventObserver observer);
    }
}