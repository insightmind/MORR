using System;
using MORR.Modules.Clipboard.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.Clipboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ClipboardPasteEvent
    /// </summary>
    public class ClipboardPasteEventProducer : DefaultEventQueue<ClipboardPasteEvent>

    {
        public void StartCapture()
        {
            GlobalHook.IsActive = true;
            GlobalHook.AddListener(GlobalHookCallBack, NativeMethods.MessageType.WM_PASTE);
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(GlobalHookCallBack, NativeMethods.MessageType.WM_PASTE);
        }


        #region private methods

        /// <summary>
        ///     The callback for the clipboard hook
        /// </summary>
        /// <param name="nCode">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The clipboard cut event hook information</param>
        /// <returns></returns>
        private void GlobalHookCallBack(GlobalHook.HookMessage message)
        {
            var text = NativeMethods.getClipboardText();

            //create the corresponding new Event
            var clipboardPasteEvent = new ClipboardPasteEvent { Text = text };

            //enqueue the new event.
            Enqueue(clipboardPasteEvent);
        }

        #endregion
    }
}