using System;
using System.ComponentModel.Composition;
using MORR.Modules.Clipboard.Native;
using MORR.Modules.Clipboard.Producers;
using MORR.Shared.Modules;
using MORR.Shared.Utility;

namespace MORR.Modules.Clipboard
{
    /// <summary>
    ///     The <see cref="ClipboardModule" /> is responsible for recording all clipboard related user interactions
    /// </summary>
    public class ClipboardModule : IModule
    {
        private bool isActive;

        /// <summary>
        ///     A single-writer-multiple-reader queue for ClipboardCopyEvent
        /// </summary>
        [Import]
        private ClipboardCopyEventProducer ClipboardCopyEventProducer { get; set; }

        /// <summary>
        ///     A single-writer-multiple-reader queue for ClipboardCutEvent
        /// </summary>
        [Import]
        private ClipboardCutEventProducer ClipboardCutEventProducer { get; set; }

        /// <summary>
        ///     A single-writer-multiple-reader queue for ClipboardPasteEvent
        /// </summary>
        [Import]
        private ClipboardPasteEventProducer ClipboardPasteEventProducer { get; set; }

        /// <summary>
        ///     Unique module identifier
        /// </summary>
        public static Guid Identifier { get; } = new Guid("B9179D3D-2DB4-46FA-845E-B47F9DCF7745");

        Guid IModule.Identifier => Identifier;

        /// <summary>
        ///     Gets or sets the value indicating whether the module is active in current recording session
        ///     Hooks clipboard events, when the module is enabled
        ///     Unhooks clipboard events, when the module is disabled
        /// </summary>
        public bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, StartCapture, StopCapture);
        }

        public void Initialize(bool isEnable)
        {
            if (isEnable)
            {
                ClipboardCutEventProducer?.Open();
                ClipboardPasteEventProducer?.Open();
                ClipboardCopyEventProducer?.Open();
            }
            else
            {
                ClipboardCutEventProducer?.Close();
                ClipboardPasteEventProducer?.Close();
                ClipboardCopyEventProducer?.Close();
            }
        }

        private void StartCapture()
        {
            IClipboardWindowMessageSink clipboardWindowMessageSink = new ClipboardWindowMessageSink();
            INativeClipboard nativeCb = new NativeClipboard();
            ClipboardCutEventProducer?.StartCapture(clipboardWindowMessageSink, nativeCb);
            ClipboardPasteEventProducer?.StartCapture(nativeCb);
            ClipboardCopyEventProducer?.StartCapture(clipboardWindowMessageSink, nativeCb);
        }

        private void StopCapture()
        {
            ClipboardCutEventProducer?.StopCapture();
            ClipboardPasteEventProducer?.StopCapture();
            ClipboardCopyEventProducer?.StopCapture();
        }
    }
}