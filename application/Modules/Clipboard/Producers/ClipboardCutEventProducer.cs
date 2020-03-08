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
        private static readonly ClipboardWindowMessageSink clipboardWindowMessageSink = new ClipboardWindowMessageSink();
        private INativeClipboard nativeClipboard = clipboardWindowMessageSink.NativeClipboard;

        #region Private methods

        private void OnClipboardUpdate(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
        {
            if (messageId != (int)GlobalHook.MessageType.WM_CLIPBOARDUPDATE)
            {
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

            if (Convert.ToString(wParam) == "0" || Convert.ToString(wParam) == "14")
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