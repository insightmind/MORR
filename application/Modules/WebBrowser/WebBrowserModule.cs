using System;
using MORR.Shared.Modules;
using System.Composition;
using MORR.Modules.WebBrowser.Producers;

namespace MORR.Modules.WebBrowser
{
    /// <summary>
    /// The <see cref="WebBrowserModule"/> is responsible for recording all browser related user interactions
    /// </summary>
    public class WebBrowserModule : ICollectingModule
    {
        public bool IsActive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid Identifier => throw new NotImplementedException();

        /// <summary>
        /// A single-writer-multiple-reader queue for ButtonClickEvent
        /// </summary>
        [Import] 
        public ButtonClickEventProducer ButtonClickEventProducer { get; private set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for CloseTabEvent
        /// </summary>
        [Import]
        public CloseTabEventProducer CloseTabEventProducer { get; private set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for FileDownloadEvent
        /// </summary> 
        [Import]
        public FileDownloadEventProducer FileDownloadEventProducer { get; private set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for HoverEvent
        /// </summary>
        [Import]
        public HoverEventProducer HoverEventProducer { get; private set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for NavigationEvent
        /// </summary>
        [Import]
        public NavigationEventProducer NavigationEventProducer { get; private set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for OpenTabEvent
        /// </summary>
        [Import]
        public OpenTabEventProducer OpenTabEventProducer { get; private set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for SwitchTabEvent
        /// </summary>
        [Import]
        public SwitchTabEventProducer SwitchTabEventProducer { get; private set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for TextInputEvent
        /// </summary>
        [Import]
        public TextInputEventProducer TextInputEventProducer { get; private set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for TextSelectionEvent
        /// </summary>
        [Import]
        public TextSelectionEventProducer TextSelectionEventProducer { get; private set; }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
