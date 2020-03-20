using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using MORR.Modules.WebBrowser.Producers;
using MORR.Shared.Modules;
using MORR.Shared.Utility;

[assembly: InternalsVisibleTo("WebBrowserTest")]
namespace MORR.Modules.WebBrowser
{
    /// <summary>
    ///     The <see cref="WebBrowserModule" /> is responsible for recording all browser related user interactions
    /// </summary>
    public class WebBrowserModule : IModule
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS0649 // Fields is never assigned to, and will always have its default value null
        [Import] private ButtonClickEventProducer buttonClickEventProducer;

        [Import] private CloseTabEventProducer closeTabEventProducer;

        [Import] private FileDownloadEventProducer fileDownloadEventProducer;

        [Import] private HoverEventProducer hoverEventProducer;

        [Import] private NavigationEventProducer navigationEventProducer;

        [Import] private OpenTabEventProducer openTabEventProducer;

        [Import] private SwitchTabEventProducer switchTabEventProducer;

        [Import] private TextInputEventProducer textInputEventProducer;

        [Import] private TextSelectionEventProducer textSelectionEventProducer;
#pragma warning restore CS0649 // Fields is never assigned to, and will always have its default value null

        [Import] private WebBrowserModuleConfiguration Configuration { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        private bool isActive;
        private WebExtensionListener? listener;
        private List<IWebBrowserEventObserver>? producers;

        public static Guid Identifier { get; } = new Guid("e9240dc4-f33f-43db-a419-5b36d8279c88");
        Guid IModule.Identifier => Identifier;

        public bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, StartRecording, StopRecording);
        }

        private void StartRecording()
        {
            if (listener == null)
            {
                return;
            }

            listener.RecordingActive = true;
        }

        private void StopRecording()
        {
            if (listener == null)
            {
                return;
            }

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

            if (isEnabled)
            {
                if (listener == null)
                {
                    listener = new WebExtensionListener(Configuration.UrlSuffix);
                }

                foreach (var producer in producers)
                {
                    listener.Subscribe(producer, producer.HandledEventLabel);
                }
                listener.StartListening();
                producers.ForEach(producer => producer.Open());
            }
            else
            {
                producers.ForEach(producer => producer.Close());
            }
        }

        //revert back to unitialized state, only needed for (unit-)testing
        internal void Reset()
        {
            if (listener != null)
            {
                listener.StopListening();
                listener = null;
            }
            producers = null;
        }
    }
}