using System;
using MORR.Shared.Modules;
using MORR.Modules.Clipboard.Producers;
using System.Composition;


namespace MORR.Modules.Clipboard
{
    /// <summary>
    /// The <see cref="ClipboardModule"/> is responsible for recording all clipboard related user interactions
    /// </summary>
    public class ClipboardModule : ICollectingModule
    {
        private bool isEnabled;

        /// <summary>
        /// A single-writer-multiple-reader queue for ClipboardCopyEvent
        /// </summary>
        [Import]
        private ClipboardCopyEventProducer ClipboardCopyEventProducer { get; set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for ClipboardCutEvent
        /// </summary>
        [Import]
        private ClipboardCutEventProducer ClipboardCutEventProducer { get; set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for ClipboardPasteEvent
        /// </summary>
        [Import]
        private ClipboardPasteEventProducer ClipboardPasteEventProducer { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the module is enabled in current recording session
        /// Hooks clipboard events, when the module is enabled
        /// Unhooks clipboard events, when the module is disabled
        /// </summary>
        public bool IsEnabled 
        { 
            get => isEnabled;
            set
            {
                isEnabled = value;
                if (isEnabled)
                {
                    ClipboardCopyEventProducer.HookClipboardCopyEvents();
                    ClipboardCutEventProducer.HookClipboardCutEvent();
                    ClipboardPasteEventProducer.HookClipboardPasteEvent();
                }
                else
                {
                    ClipboardCopyEventProducer.UnhookClipboardCopyEvents();
                    ClipboardCutEventProducer.UnhookClipboardCutEvent();
                    ClipboardPasteEventProducer.UnhookClipboardPasteEvent();
                }
            }
        }
        /// <summary>
        /// Unique module identifier
        /// </summary>
        public Guid Identifier => new Guid("06618216-80DC-4866-8717-BCBBEDB43BFB");

        public void Initialize()
        {
        }
    }
}
