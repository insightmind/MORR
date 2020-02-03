using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using MORR.Modules.WebBrowser.Producers;
using MORR.Shared.Modules;
using MORR.Shared.Utility;

namespace MORR.Modules.WebBrowser
{
    /// <summary>
    ///     The <see cref="WebBrowserModule" /> is responsible for recording all browser related user interactions
    /// </summary>
    public class WebBrowserModule : ICollectingModule
    {
        [Import] private ButtonClickEventProducer buttonClickEventProducer;

        [Import] private CloseTabEventProducer closeTabEventProducer;

        [Import] private FileDownloadEventProducer fileDownloadEventProducer;

        [Import] private HoverEventProducer hoverEventProducer;

        [Import] private NavigationEventProducer navigationEventProducer;

        [Import] private OpenTabEventProducer openTabEventProducer;

        [Import] private SwitchTabEventProducer switchTabEventProducer;

        [Import] private TextInputEventProducer textInputEventProducer;

        [Import] private TextSelectionEventProducer textSelectionEventProducer;
        [Import] private WebBrowserModuleConfiguration Configuration { get; set; }

        private bool isActive;
        private WebExtensionListener listener;
        private List<IWebBrowserEventObserver> producers;

        public static Guid Identifier { get; } = new Guid("e9240dc4-f33f-43db-a419-5b36d8279c88");
        Guid IModule.Identifier => Identifier;

        public bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value,
                                          () => listener.RecordingActive = true,
                                          () => listener.RecordingActive = false);
        }

        public void Initialize()
        {
            producers = new List<IWebBrowserEventObserver>
            {
                buttonClickEventProducer, closeTabEventProducer, fileDownloadEventProducer,
                hoverEventProducer, navigationEventProducer, openTabEventProducer,
                switchTabEventProducer, textInputEventProducer, textSelectionEventProducer
            };
            listener = new WebExtensionListener(Configuration.UrlSuffix);
            listener.StartListening();
            foreach (var producer in producers)
            {
                listener.Subscribe(producer, producer.HandledEventLabel);
            }
        }
    }
}