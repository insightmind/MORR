using System;
using MORR.Shared.Modules;

namespace MORR.Modules.WebBrowser
{
    /// <summary>
    /// The <see cref="WebBrowserModule"/> is responsible for recording all browser related user interactions
    /// </summary>
    public class WebBrowserModule : ICollectingModule
    {
        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid Identifier => throw new NotImplementedException();

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
