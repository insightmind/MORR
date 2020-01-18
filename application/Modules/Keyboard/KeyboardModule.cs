using System;
using MORR.Shared.Modules;
using MORR.Modules.Keyboard.Producers;
using System.Composition;

namespace MORR.Modules.Keyboard
{
    /// <summary>
    /// The <see cref="KeyboardModule"/> is responsible for recording all keyboard related user interactions
    /// </summary>
    public class KeyboardModule : ICollectingModule
    {
        private bool isEnabled;
        /// <summary>
        /// if the module is enabled or not.
        /// When a module is being enabled, the keyboard hook will be set.
        /// When a module is being disabled, the keyboard hook will be released.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                isEnabled = value;
                if (value)
                {
                    keyboardInteractEventProducer.HookKeyboard();
                }
                else
                {
                    keyboardInteractEventProducer.UnhookKeyboard();
                }
            }
        }

        private KeyboardInteractEventProducer keyboardInteractEventProducer;
        /// <summary>
        /// A single-writer-multiple-reader queue for KeyboardInteractEvent
        /// </summary>
        [Import]
        public KeyboardInteractEventProducer KeyboardInteractEventProducer 
        {
            get 
            {
                return this.keyboardInteractEventProducer;
            }
            private set
            {
                this.keyboardInteractEventProducer = value;
            }
        }

        public Guid Identifier => throw new NotImplementedException();


        /// <summary>
        ///     Initialize the module unenabled with KeyboardInteractEventProducer.
        /// </summary>
        public void Initialize()
        {
            this.keyboardInteractEventProducer = new KeyboardInteractEventProducer();
            this.isEnabled = false;
        }
    }
}
