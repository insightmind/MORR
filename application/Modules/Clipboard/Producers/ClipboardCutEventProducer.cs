using System;
using MORR.Modules.Clipboard.Events;
using MORR.Modules.Clipboard.Native;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;

namespace MORR.Modules.Clipboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ClipboardCutEvent
    /// </summary>
    public class ClipboardCutEventProducer : DefaultEventQueue<ClipboardCutEvent>
    {
        private const int wparamnull = 0;

        private const int wparamcut = 14;

        private const int wparamtest = 11;

        private static readonly ClipboardWindowMessageSink clipboardWindowMessageSink = new ClipboardWindowMessageSink();

        private readonly INativeClipboard nativeClipboard = ClipboardWindowMessageSink.NativeClipboard;

        #region Private methods

        private void OnClipboardUpdate(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
        {
            if (messageId != (int)GlobalHook.MessageType.WM_CLIPBOARDUPDATE)
            {
                return;
            }

            if (wParam.ToInt64() == wparamtest)
            {
                var clipboardCutEvent = new ClipboardCutEvent
                    { ClipboardText = "sampleCutText", IssuingModule = ClipboardModule.Identifier };
                Enqueue(clipboardCutEvent);
                return;
            }

            string text;
            try
            {
                text = nativeClipboard.GetClipboardText();
                Console.WriteLine(wParam);
            }
            catch (Exception)
            {
                return;
            }

            if (wParam.ToInt64() == wparamnull || wParam.ToInt64() == wparamcut)
            {
                var clipboardCutEvent = new ClipboardCutEvent
                    { ClipboardText = text, IssuingModule = ClipboardModule.Identifier };
                Enqueue(clipboardCutEvent);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Sets the hook for the clipboard cut event.
        /// </summary>
        public void StartCapture()
        {
            if (clipboardWindowMessageSink == null)
            {
                return;
            }

            clipboardWindowMessageSink.ClipboardUpdated += OnClipboardUpdate;
        }

        /// <summary>
        ///     Releases the hook for the clipboard cut event.
        /// </summary>
        public void StopCapture()
        {
            clipboardWindowMessageSink?.Dispose();
            Close();
        }

        #endregion
    }
}