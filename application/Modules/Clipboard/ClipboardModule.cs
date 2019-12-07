using System;
using MORR.Shared.Modules;

namespace MORR.Modules.Clipboard
{
    /// <summary>
    /// The <see cref="ClipboardModule"/> is responsible for recording all clipboard related user interactions
    /// </summary>
    public class ClipboardModule : ICollectingModule
    {
        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid Identifier => throw new NotImplementedException();

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
