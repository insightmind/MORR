using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using MORR.Shared.Modules;
using MORR.Shared.Utility;

namespace MORR.Modules.WebBrowser
{
    /// <summary>
    ///     The <see cref="WebBrowserModule" /> is responsible for recording all browser related user interactions
    /// </summary>
    [Export(typeof(IModule))]
    public class WebBrowserModule : ICollectingModule
    {
        private bool isActive;
        private WebExtensionListener listener;


        [ImportMany]
        private IEnumerable<IWebBrowserEventObserver> Producers { get; set; }


        [Import]
        private WebBrowserModuleConfiguration Configuration { get; set; }

        public bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value,
                                          () => listener.RecordingActive = true,
                                          () => listener.RecordingActive = false);
        }

        public Guid Identifier { get; } = new Guid("e9240dc4-f33f-43db-a419-5b36d8279c88");

        public void Initialize()
        {
            listener = new WebExtensionListener(Configuration.UrlSuffix);
            listener.startListening();
            foreach (var producer in Producers)
            {
                listener.Subscribe(producer, producer.HandledEventType);
            }
        }
    }
}