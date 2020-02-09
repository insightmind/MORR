﻿using System;
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
    public class WebBrowserModule : IModule
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
            set => Utility.SetAndDispatch(ref isActive, value, StartRecording, StopRecording);
        }

        private void StartRecording()
        {
            listener.RecordingActive = true;
        }

        private void StopRecording()
        {
            listener.RecordingActive = false;
            producers?.ForEach(producer => producer.Close());
        }

        public void Initialize(bool isEnabled)
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

            if (isEnabled)
            {
                producers.ForEach(producer => producer.Open());
            }
            else
            {
                producers.ForEach(producer => producer.Close());
            }
        }
    }
}