using System;
using MORR.Shared.Modules;

namespace MORR.Modules.WindowManagement
{
    /// <summary>
    /// The <see cref="WindowManagementModule"/> is responsible for recording all window related user interactions
    /// </summary>
    public class WindowManagementModule : ICollectingModule
    {
        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid Identifier => throw new NotImplementedException();

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
