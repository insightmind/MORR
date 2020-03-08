using System;
using MORR.Modules.Clipboard.Events;
using MORR.Modules.Clipboard.Native;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;

namespace MORR.Modules.Clipboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ClipboardCopyEvent
    /// </summary>
    public class ClipboardCopyEventProducer : DefaultEventQueue<ClipboardCopyEvent>
    {
        /// <summary>
        ///!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        ///     When I initialize the nativeClipboard in the StartCapture() method
        ///     like i did in the other producers, an Exception will be thrown.
        ///     ERROR: The type initializer for
        ///     'MORR.Modules.Clipboard.Producers.ClipboardCopyEventProducer'
        ///     threw an exception.
        ///!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 
        /// </summary>

        private static readonly ClipboardWindowMessageSink
            clipboardWindowMessageSink = new ClipboardWindowMessageSink();

        private INativeClipboard nativeClipboard = clipboardWindowMessageSink.NativeClipboard;

        #region Private methods

        private void OnClipboardUpdate(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
        {
            if (messageId != (int) GlobalHook.MessageType.WM_CLIPBOARDUPDATE)
            {
                return;
            }

            string text;
            try
            {
                text = nativeClipboard.GetClipboardText();
            }
            catch (Exception)
            {
                return;
            }

            if (Convert.ToString(wParam) == "0" || Convert.ToString(wParam) == "18")
            {
                var clipboardCopyEvent = new ClipboardCopyEvent
                    { ClipboardText = text, IssuingModule = ClipboardModule.Identifier };
                Enqueue(clipboardCopyEvent);
            } 
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Sets the hook for the clipboard copy event.
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
        ///     Releases the hook for the clipboard copy event.
        /// </summary>
        public void StopCapture()
        {
            clipboardWindowMessageSink?.Dispose();
            Close();
        }

        #endregion
    }
}