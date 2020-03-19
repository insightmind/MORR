using System;

namespace MORR.Modules.Clipboard.Native
{
    public interface IClipboardWindowMessageSink
    {
        /// <summary>
        ///     Handles a window message when clipboard is updated
        /// </summary>
        /// <param name="hwnd">The pointer to the current window</param>
        /// <param name="uMsg">The identifier of the message</param>
        /// <param name="wParam">The WPARAM of the message</param>
        /// <param name="lParam">The LPARAM of the message</param>
        public delegate void ClipboardEventHandler(IntPtr hwnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        IntPtr OnClipboardUpdate(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam);

        void Dispose();

        public event ClipboardEventHandler? ClipboardUpdated;
    }
}