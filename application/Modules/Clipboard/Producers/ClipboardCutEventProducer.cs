using MORR.Modules.Clipboard.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.Clipboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ClipboardCutEvent
    /// </summary>
    public class ClipboardCutEventProducer : DefaultEventQueue<ClipboardCutEvent>

    {
        public void StartCapture()
        {
            Open();
            GlobalHook.IsActive = true;
            GlobalHook.AddListener(GlobalHookCallBack, NativeMethods.MessageType.WM_CUT);
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(GlobalHookCallBack, NativeMethods.MessageType.WM_CUT);
            Close();
        }


        #region private methods

        private void GlobalHookCallBack(GlobalHook.HookMessage message)
        {
            var text = NativeMethods.GetClipboardText();

            //create the corresponding new Event
            var clipboardCutEvent = new ClipboardCutEvent
                { ClipboardText = text, IssuingModule = ClipboardModule.Identifier };

            //enqueue the new event.
            Enqueue(clipboardCutEvent);
        }

        #endregion
    }
}