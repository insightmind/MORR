using System;
using MORR.Shared.Modules;

namespace MORR.Modules.Mouse
{
    /// <summary>
    /// The <see cref="MouseModule"/> is responsible for recording all mouse related user interactions
    /// </summary>
    public class MouseModule : ICollectingModule
    {
        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid Identifier => throw new NotImplementedException();

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
