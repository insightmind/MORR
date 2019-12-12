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
        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid Identifier => throw new NotImplementedException();

        /// <summary>
        /// A single-writer-multiple-reader queue for KeyboardInteractEvent
        /// </summary>
        [Import]
        public KeyboardInteractEventProducer KeyboardInteractEventProducer { get; private set; }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
