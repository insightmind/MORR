using System.ComponentModel.Composition;
using MORR.Modules.Mouse.Events;
using MORR.Shared.Events;
using System.Composition;
using System.Diagnostics;

namespace MORR.Modules.Mouse.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for MouseMoveEvent
    /// </summary>
    [Export(typeof(MouseMoveEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<MouseMoveEvent>))]
    public class MouseMoveEventProducer : DefaultEventQueue<MouseMoveEvent>
    {
        #region private fields
        private IntPtr hook = IntPtr.Zero;


        #endregion

        #region constructor



        #endregion



        #region NativeMethods
        private static class NativeMethods
        {
            #region #region Constant, Structure and Delegate Definitions



            #endregion



            #region DLL imports



            #endregion
        }
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