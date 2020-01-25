using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using MORR.Modules.WebBrowser.Events;

namespace MORR.Modules.WebBrowser
{
    internal interface IWebBrowserEventObserver
    {
        public void Notify(WebBrowserEvent @event);
        public Type HandledEventType { get; }
    }

    internal interface IWebBrowserEventObservible
    {
        public void SubScribe(IWebBrowserEventObserver observer, Type eventType);
        public void UnSubScribe(IWebBrowserEventObserver observer);
    }
}
