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
        private KeyboardInteractEventProducer keyboardInteractEventProducer;


        public bool IsEnabled
        {
            get
            {
                return this.IsEnabled;
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

        public Guid Identifier => throw new NotImplementedException();

        /// <summary>
        /// A single-writer-multiple-reader queue for KeyboardInteractEvent
        /// </summary>
        [Import]
        public KeyboardInteractEventProducer KeyboardInteractEventProducer { get; private set; }

        public void Initialize()
        {
            this.keyboardInteractEventProducer = new KeyboardInteractEventProducer;
        }
    }
}
