﻿using System.ComponentModel.Composition;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events;
using System.Composition;
using System.Diagnostics;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseClickEvent
    /// </summary>
    [Export(typeof(MouseClickEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<MouseClickEvent>))]
    public class MouseClickEventProducer : DefaultEventQueue<MouseClickEvent>
    {
        #region private fields
        private IntPtr hook = IntPtr.Zero;


        #endregion

        #region constructor



        #endregion

        #region public methods
        /// <summary>
        ///     Set the hook for the Mouse.
        /// </summary>
        public void HookMouse()
        {
            CommonMethods.HookMouse(hook);
        }

        /// <summary>
        ///     Release the hook for the keyboard.
        /// </summary>
        public void UnhookKeyboard()
        {
            CommonMethods.UnhookKeyboard(hook);
        }


        #endregion

        #region private methods



        #endregion
    }
}