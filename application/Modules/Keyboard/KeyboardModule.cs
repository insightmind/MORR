using System;
using MORR.Shared.Modules;

namespace MORR.Modules.Keyboard
{
    /// <summary>
    /// The <see cref="KeyboardModule"/> is responsible for recording all keyboard related user interactions
    /// </summary>
    public class KeyboardModule : ICollectingModule
    {
        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid Identifier => throw new NotImplementedException();

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
