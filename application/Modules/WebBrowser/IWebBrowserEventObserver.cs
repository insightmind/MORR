using System;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser
{
    internal interface IWebBrowserEventObserver
    {
        public Type HandledEventType { get; }
        public void Notify(WebBrowserEvent @event);
    }

    internal interface IWebBrowserEventObservable
    {
        public void Subscribe(IWebBrowserEventObserver observer, Type eventType);
        public void Unsubscribe(IWebBrowserEventObserver observer);
    }
}