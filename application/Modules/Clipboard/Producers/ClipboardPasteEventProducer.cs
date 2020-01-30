using System;
using System.ComponentModel.Composition;
using MORR.Modules.Clipboard.Events;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Modules.Clipboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ClipboardPasteEvent
    /// </summary>
    [Export(typeof(ClipboardPasteEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<ClipboardPasteEvent>))]
    [Export(typeof(IReadWriteEventQueue<ClipboardPasteEvent>))]
    [Export(typeof(IReadOnlyEventQueue<Event>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ClipboardPasteEventProducer : DefaultEventQueue<ClipboardPasteEvent>

    {
        private NativeMethods.LowLevelClipboardProc? callback;
        private IntPtr clipboardHookHandle;

        public void StartCapture()
        {
            callback = ClipboardHookCallback; // Store callback to prevent GC
            if (!NativeMethods.TrySetClipboardPasteHook(callback, out clipboardHookHandle))
            {
                throw new Exception("Failed hook clipboard.");
            }
        }

        public void StopCapture()
        {
            if (!NativeMethods.UnhookWindowsHookEx(clipboardHookHandle))
            {
                throw new Exception("Failed to unhook clipboard.");
            }
        }


        #region private methods

        /// <summary>
        ///     The callback for the clipboard hook
        /// </summary>
        /// <param name="nCode">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The clipboard cut event hook information</param>
        /// <returns></returns>
        private int ClipboardHookCallback(int nCode, NativeMethods.MessageType wParam, int lParam)
        {
            if (nCode < 0)
            {
                return NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            }

            if (wParam == NativeMethods.MessageType.WM_PASTE)
            {
                var text = NativeMethods.getClipboardText();

                //create the corresponding new Event
                var clipboardPasteEvent = new ClipboardPasteEvent { Text = text };

                //enqueue the new event.
                Enqueue(clipboardPasteEvent);
            }

            return NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        #endregion
    }
}