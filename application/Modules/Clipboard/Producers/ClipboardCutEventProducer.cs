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
        private const int wparamcut = 14;

        private static IClipboardWindowMessageSink? clipboardWindowMessageSink;

        private static INativeClipboard? nativeClipboard;

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
                if (nativeClipboard == null)
                {
                    return;
                }

                text = nativeClipboard.GetClipboardText();
            }
            catch (Exception)
            {
                return;
            }

            if (wParam.ToInt64() == wparamcut)
            {
                var clipboardCutEvent = new ClipboardCutEvent
                    { ClipboardText = text, IssuingModule = ClipboardModule.Identifier };
                Enqueue(clipboardCutEvent);
            }
        }

        private void GlobalHookCallBack(GlobalHook.HookMessage message)
        {
            string text;
            try
            {
                if (nativeClipboard == null)
                {
                    return;
                }

                text = nativeClipboard.GetClipboardText();
            }
            catch (Exception)
            {
                return;
            }

            //create the corresponding new Event
            var clipboardCutEvent = new ClipboardCutEvent
                { ClipboardText = text, IssuingModule = ClipboardModule.Identifier };

            //enqueue the new event.
            Enqueue(clipboardCutEvent);
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Sets the hook for the clipboard cut event.
        /// </summary>
        public void StartCapture(IClipboardWindowMessageSink windowMessageSink, INativeClipboard nativeClip)
        {
            clipboardWindowMessageSink = windowMessageSink;
            nativeClipboard = nativeClip;

            if (clipboardWindowMessageSink == null)
            {
                return;
            }

            clipboardWindowMessageSink.ClipboardUpdated += OnClipboardUpdate;

            GlobalHook.IsActive = true;
            GlobalHook.AddListener(GlobalHookCallBack, GlobalHook.MessageType.WM_CUT);
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