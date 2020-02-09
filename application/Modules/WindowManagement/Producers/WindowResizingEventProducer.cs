﻿using System.Drawing;
using MORR.Modules.WindowManagement.Events;
using MORR.Modules.WindowManagement.Native;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowResizingEvent
    /// </summary>
    public class WindowResizingEventProducer : DefaultEventQueue<WindowResizingEvent>
    {
        /// <summary>
        ///     The rectangle that holds the size and location of the window
        ///     after the change.
        /// </summary>
        private Rectangle windowRecAfterChange;

        /// <summary>
        ///     The rectangle that holds the size and location of the window
        ///     before the change.
        /// </summary>
        private Rectangle windowRecBeforeChange;

        /// <summary>
        ///     The hwnd of the windows being changed.
        ///     Change can mean move or resize.
        /// </summary>
        private int windowUnderChangeHwnd;

        private readonly GlobalHook.MessageType[] listenedMessageTypes = { GlobalHook.MessageType.WM_ENTERSIZEMOVE, GlobalHook.MessageType.WM_EXITSIZEMOVE };

        public void StartCapture()
        {
            GlobalHook.AddListener(WindowHookCallback, listenedMessageTypes);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(WindowHookCallback, listenedMessageTypes);
            Close();
        }

        /// <summary>
        ///     Everytime a WM_ENTERSIZEMOVE is received,
        ///     records the information of the window before the change
        ///     and wait for the WM_EXITSIZEMOVE.
        ///     Everytime a WM_EXITSIZEMOVE is received,
        ///     records the information of the window after the change
        ///     and see if the window has been resized.
        ///     (by if !(Utility.IsRectSizeEqual(windowRecBeforeChange,windowRecAfterChange)))
        ///     If so, records the relevant information into a WindowResizingEvent.
        /// </summary>
        /// <param name="msg">the hook message</param>
        private void WindowHookCallback(GlobalHook.HookMessage msg)
        {
            if (msg.Type == (uint) GlobalHook.MessageType.WM_ENTERSIZEMOVE)
            {
                windowUnderChangeHwnd = (int) msg.Hwnd;
                windowRecBeforeChange = new Rectangle();
                NativeWindowManagement.GetWindowRect(windowUnderChangeHwnd, ref windowRecBeforeChange);
            }

            if (msg.Type == (uint) GlobalHook.MessageType.WM_EXITSIZEMOVE)
            {
                windowRecAfterChange = new Rectangle();
                NativeWindowManagement.GetWindowRect(windowUnderChangeHwnd, ref windowRecAfterChange);
                if (!NativeWindowManagement.IsRectSizeEqual(windowRecBeforeChange, windowRecAfterChange))
                {
                    var oldSize = new Size
                    {
                        Width = NativeWindowManagement.GetWindowWidth(windowRecBeforeChange),
                        Height = NativeWindowManagement.GetWindowHeight(windowRecBeforeChange)
                    };
                    var newSize = new Size
                    {
                        Width = NativeWindowManagement.GetWindowWidth(windowRecAfterChange),
                        Height = NativeWindowManagement.GetWindowHeight(windowRecAfterChange)
                    };
                    var @event = new WindowResizingEvent
                    {
                        IssuingModule = WindowManagementModule.Identifier,
                        OldSize = oldSize,
                        NewSize = newSize,
                        Title = NativeWindowManagement.GetWindowTitleFromHwnd(msg.Hwnd),
                        ProcessName = NativeWindowManagement.GetProcessNameFromHwnd(msg.Hwnd)
                    };
                    Enqueue(@event);
                }
            }
        }
    }
}