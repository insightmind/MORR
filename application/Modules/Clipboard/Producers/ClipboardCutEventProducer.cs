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
        private static INativeClipboard nativeClipboard;

        public void StartCapture(INativeClipboard nativeCb)
        {
            nativeClipboard = nativeCb;
            GlobalHook.IsActive = true;
            GlobalHook.AddListener(GlobalHookCallBack, GlobalHook.MessageType.WM_CUT);
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(GlobalHookCallBack, GlobalHook.MessageType.WM_CUT);
            Close();
        }


        #region private methods

        private void GlobalHookCallBack(GlobalHook.HookMessage message)
        {
            var text = nativeClipboard.GetClipboardText();

            //create the corresponding new Event
            var clipboardCutEvent = new ClipboardCutEvent
                { ClipboardText = text, IssuingModule = ClipboardModule.Identifier };

            //enqueue the new event.
            Enqueue(clipboardCutEvent);
        }

        #endregion
    }
}