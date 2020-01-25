using System;
using System.Collections.Generic;
using MORR.Shared.Modules;
using MORR.Modules.WebBrowser.Events;
using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Producers;
using MORR.Shared.Utility;

namespace MORR.Modules.WebBrowser
{
    /// <summary>
    /// The <see cref="WebBrowserModule"/> is responsible for recording all browser related user interactions
    /// </summary>
    public class WebBrowserModule : ICollectingModule
    {

        private bool isActive;
        private WebExtensionListener listener;
        public bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, 
                                          () =>
                                          {
                                              listener.RecordingActive = true;
                                          },
                                          () =>
                                          {
                                              listener.RecordingActive = false;
                                          });
        }

        public Guid Identifier { get; } = new Guid("e9240dc4-f33f-43db-a419-5b36d8279c88");


        [ImportMany]
        private IEnumerable<WebBrowserEventProducer<WebBrowserEvent>> Producers { get; set; }


        [Import]
        private WebBrowserModuleConfiguration Configuration { get; set; }

        public void Initialize()
        {
            listener = new WebExtensionListener(Configuration.UrlSuffix);
            listener.startListening();
            foreach (var producer in Producers)
            {
                listener.SubScribe(producer, producer.HandledEventType);
            }
        }
    }
}
