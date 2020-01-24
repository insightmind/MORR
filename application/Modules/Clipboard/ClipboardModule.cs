using System;
using System.ComponentModel.Composition;
using MORR.Shared.Modules;
using MORR.Modules.Clipboard.Producers;

namespace MORR.Modules.Clipboard
{
    /// <summary>
    /// The <see cref="ClipboardModule"/> is responsible for recording all clipboard related user interactions
    /// </summary>
    public class ClipboardModule : ICollectingModule
    {
        public bool IsActive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid Identifier => throw new NotImplementedException();

        /// <summary>
        /// A single-writer-multiple-reader queue for ClipboardInteractEvent
        /// </summary>
        [Import]
        public ClipboardInteractEventProducer ClipboardInteractEventProducer { get; private set; }
        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
