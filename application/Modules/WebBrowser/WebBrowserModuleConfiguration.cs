using System.Composition;
using System.Text.Json;
using MORR.Shared.Configuration;
using MORR.Core.Recording;

namespace MORR.Modules.WebBrowser
{
    [Export(typeof(RecordingConfiguration))]
    [Export(typeof(IConfiguration))]
    public class WebBrowserModuleConfiguration : IConfiguration
    {
        /// <summary>
        ///     A combination of port number and optionally directory.
        ///     Will be appended to the localhost-prefix defined in <see cref="WebExtensionListener"/>.
        ///     Must end in a slash ('/') character.
        ///     Examples for valid values are "60024/" or "60024/johndoe/".
        /// </summary>
        public string UrlSuffix;
        public string Identifier => "WebBrowser";
        public void Parse(string configuration)
        {
            var instance = JsonSerializer.Deserialize<WebBrowserModuleConfiguration>(configuration);
            this.UrlSuffix = instance.UrlSuffix;
        }
    }
}
