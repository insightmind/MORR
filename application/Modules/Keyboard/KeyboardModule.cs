using System;
using System.ComponentModel.Composition;
using MORR.Modules.Keyboard.Native;
using MORR.Modules.Keyboard.Producers;
using MORR.Shared.Modules;
using MORR.Shared.Utility;

namespace MORR.Modules.Keyboard
{
    /// <summary>
    ///     The <see cref="KeyboardModule" /> is responsible for recording all keyboard related user interactions.
    /// </summary>
    public class KeyboardModule : IModule
    {
        private bool isActive;

        [Import]
        private KeyboardInteractEventProducer KeyboardInteractEventProducer { get; set; }

        /// <summary>
        ///     Indicates whether the module is active or not.
        ///     When a module is being active, the keyboard hook will be set.
        ///     When a module is being inactive, the keyboard hook will be released.
        /// </summary>
        public bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, StartCapture, StopCapture);
        }

        public static Guid Identifier { get; } = new Guid("99F679D6-0D20-40EE-8604-F128F0E5AE3B");
        Guid IModule.Identifier => Identifier;

        public void Initialize(bool isEnable)
        {
            if (isEnable)
            {
                KeyboardInteractEventProducer?.Open();
            }
            else
            {
                KeyboardInteractEventProducer?.Close();
            }
        }

        private void StartCapture()
        {
            KeyboardInteractEventProducer?.StartCapture(new NativeKeyboard());
        }

        private void StopCapture()
        {
            KeyboardInteractEventProducer?.StopCapture();
        }
    }
}