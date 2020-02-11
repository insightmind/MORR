using MORR.Modules.Clipboard.Events;
using MORR.Modules.Clipboard.Native;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;
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
            GlobalHook.AddListener(GlobalHookCallBack, GlobalHook.MessageType.WM_PASTE);
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(GlobalHookCallBack, GlobalHook.MessageType.WM_PASTE);
            Close();
        }


        #region private methods

        private void GlobalHookCallBack(GlobalHook.HookMessage message)
        {
            var text = ClipboardNativeMethods.GetClipboardText();

            //create the corresponding new Event
            var clipboardPasteEvent = new ClipboardPasteEvent
                { ClipboardText = text, IssuingModule = ClipboardModule.Identifier };

            //enqueue the new event.
            Enqueue(clipboardPasteEvent);
        }

        #endregion
    }
}