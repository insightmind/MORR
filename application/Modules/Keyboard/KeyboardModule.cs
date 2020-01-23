using System;
using System.Composition;
using MORR.Modules.Keyboard.Producers;
using MORR.Shared.Modules;

namespace MORR.Modules.Keyboard
{
    /// <summary>
    ///     The <see cref="KeyboardModule" /> is responsible for recording all keyboard related user interactions
    /// </summary>
    public class KeyboardModule : ICollectingModule
    {
        private bool IsActive;

        /// <summary>
        ///     A single-writer-multiple-reader queue for KeyboardInteractEvent
        /// </summary>
        [Import]
        private KeyboardInteractEventProducer KeyboardInteractEventProducer { get; set; }

        /// <summary>
        ///     if the module is enabled or not.
        ///     When a module is being enabled, the keyboard hook will be set.
        ///     When a module is being disabled, the keyboard hook will be released.
        /// </summary>
        public bool IsEnabled
        {
            get => IsActive;
            set
            {
                IsActive = value;
                if (value)
                {
                    KeyboardInteractEventProducer.HookKeyboard();
                }
                else
                {
                    KeyboardInteractEventProducer.UnhookKeyboard();
                }
            }
        }

        public Guid Identifier => new Guid("99F679D6-0D20-40EE-8604-F128F0E5AE3B");

        bool IModule.IsActive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        /// <summary>
        ///     Initialize the module unenabled with KeyboardInteractEventProducer.
        /// </summary>
        public void Initialize()
        {
            KeyboardInteractEventProducer = new KeyboardInteractEventProducer();
            IsActive = false;
        }
    }
}