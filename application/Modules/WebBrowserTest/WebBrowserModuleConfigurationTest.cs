using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Configuration;
using MORR.Modules.WebBrowser;
using MORR.Shared.Configuration;

namespace WebBrowserTest
{
    [TestClass]
    public class WebBrowserModuleConfigurationTest
    {
        private WebBrowserModuleConfiguration config;

        [TestInitialize]
        public void TestInit()
        {
            config = new WebBrowserModuleConfiguration();
        }

        [TestMethod]
        public void ParseTest()
        {
            config.Parse(new RawConfiguration("{\r\n    \"UrlSuffix\": \"60024/\"\r\n  }"));
            Assert.AreEqual("60024/", config.UrlSuffix);
        }

        [TestMethod]
        public void ParseInvalidTest()
        {
            Assert.ThrowsException<InvalidConfigurationException>(
                () => config.Parse(new RawConfiguration("{ }")));
        }
    }
}