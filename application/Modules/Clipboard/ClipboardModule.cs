using System;
using System.ComponentModel.Composition;
using MORR.Shared.Modules;
using MORR.Modules.Clipboard.Producers;
using MORR.Shared.Utility;

namespace MORR.Modules.Clipboard
{
    /// <summary>
    /// The <see cref="ClipboardModule"/> is responsible for recording all clipboard related user interactions
    /// </summary>
    [Export(typeof(IModule))]
    public class ClipboardModule : ICollectingModule
    {
        private bool isActive;

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
        /// Gets or sets the value indicating whether the module is active in current recording session
        /// Hooks clipboard events, when the module is enabled
        /// Unhooks clipboard events, when the module is disabled
        /// </summary>
        public bool IsActive
        { 
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, startCapture,
                                          stopCapture);
        }

        private void startCapture()
        {
            ClipboardCutEventProducer.StartCapture();
            ClipboardPasteEventProducer.StartCapture();
            ClipboardCopyEventProducer.StartCapture();
        }

        private void stopCapture()
        {
            ClipboardCutEventProducer.StopCapture();
            ClipboardPasteEventProducer.StopCapture();
            ClipboardCopyEventProducer.StopCapture();
        }

        /// <summary>
        /// Unique module identifier
        /// </summary>
        public static Guid Identifier => new Guid("06618216-80DC-4866-8717-BCBBEDB43BFB");

        Guid IModule.Identifier => Identifier;

        public void Initialize() {}
    }
}
