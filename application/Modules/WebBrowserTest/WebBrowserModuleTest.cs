using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.WebBrowser;
using MORR.Modules.WebBrowser.Producers;

namespace WebBrowserTest
{
    [TestClass]
    public class WebBrowserModuleTest
    {
        private Mock<ButtonClickEventProducer> buttonClickEventProducer;
        private Mock<CloseTabEventProducer> closeTabEventProducer;
        private CompositionContainer container;
        private Mock<FileDownloadEventProducer> fileDownloadEventProducer;
        private Mock<HoverEventProducer> hoverEventProducer;
        private Mock<NavigationEventProducer> navigationEventProducer;
        private Mock<OpenTabEventProducer> openTabEventProducer;
        private Mock<SwitchTabEventProducer> switchTabEventProducer;
        private WebBrowserModuleConfiguration testWebBrowserModuleConfiguration;
        private Mock<TextInputEventProducer> textInputEventProducer;
        private Mock<TextSelectionEventProducer> textSelectionEventProducer;
        private WebBrowserModule webBrowserModule;
        private static WebBrowserModuleConfiguration config = new TestWebBrowserModuleConfiguration();
        private static HttpClient testClient;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            InitializeHTTPClient(config.UrlSuffix);
        }
        [TestCleanup]
        public void AfterTest()
        {
            webBrowserModule.Reset();
        }
        [TestInitialize]
        public void BeforeTest()
        {
            webBrowserModule = new WebBrowserModule();
            buttonClickEventProducer = new Mock<ButtonClickEventProducer>();
            closeTabEventProducer = new Mock<CloseTabEventProducer>();
            fileDownloadEventProducer = new Mock<FileDownloadEventProducer>();
            hoverEventProducer = new Mock<HoverEventProducer>();
            navigationEventProducer = new Mock<NavigationEventProducer>();
            openTabEventProducer = new Mock<OpenTabEventProducer>();
            switchTabEventProducer = new Mock<SwitchTabEventProducer>();
            textInputEventProducer = new Mock<TextInputEventProducer>();
            textSelectionEventProducer = new Mock<TextSelectionEventProducer>();
            testWebBrowserModuleConfiguration = new TestWebBrowserModuleConfiguration();
            container = new CompositionContainer();

            container.ComposeExportedValue(buttonClickEventProducer.Object);
            container.ComposeExportedValue(closeTabEventProducer.Object);
            container.ComposeExportedValue(fileDownloadEventProducer.Object);
            container.ComposeExportedValue(hoverEventProducer.Object);
            container.ComposeExportedValue(navigationEventProducer.Object);
            container.ComposeExportedValue(openTabEventProducer.Object);
            container.ComposeExportedValue(switchTabEventProducer.Object);
            container.ComposeExportedValue(textInputEventProducer.Object);
            container.ComposeExportedValue(textSelectionEventProducer.Object);
            container.ComposeExportedValue(testWebBrowserModuleConfiguration);
            container.ComposeParts(webBrowserModule);
        }

        [TestMethod]
        public void TestWebBrowserModule_Activate()
        {
            // Preconditions
            Debug.Assert(webBrowserModule != null);

            /* GIVEN */

            /* WHEN */
            webBrowserModule.Initialize(true);
            webBrowserModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(webBrowserModule.IsActive);
        }

        [TestMethod]
        public void TestWebbrowserModule_Deactivate()
        {
            // Preconditions
            Debug.Assert(webBrowserModule != null);

            /* GIVEN */

            /* WHEN */
            webBrowserModule.IsActive = false;

            /* THEN */
            Assert.IsFalse(webBrowserModule.IsActive);
        }

        [TestMethod]
        public async Task SendConnectionRequest()
        {
            webBrowserModule.Initialize(true);
            webBrowserModule.IsActive = true;
            var result = await SendHTTPMessage("{\"Request\":\"Connect\"}");

            Assert.AreEqual("Ok", result.GetProperty("response").GetString());
        }

        private static JsonElement GetJsonFromString(string data)
        {
            return JsonDocument.Parse(data).RootElement;
        }

        private class TestWebBrowserModuleConfiguration : WebBrowserModuleConfiguration
        {
            public TestWebBrowserModuleConfiguration()
            {
                UrlSuffix = "60024/";
            }
        }

        private static void InitializeHTTPClient(string UrlSuffix)
        {
            testClient = new HttpClient();
            Uri requestUri = new Uri("http://localhost:" + UrlSuffix);
            testClient.BaseAddress = requestUri;
            testClient.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
        }

        private async Task<JsonElement> SendHTTPMessage(string data)
        {
            Uri requestUri = new Uri("http://localhost:" + config.UrlSuffix);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "relativeAddress");
            /*request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}",
                                                Encoding.UTF8,
                                                "application/json");//CONTENT-TYPE header */
            request.Content = new StringContent(data,
                                                 Encoding.UTF8,
                                                "application/json");//CONTENT-TYPE header

            var response = await testClient.PostAsync(requestUri, request.Content);
            string result = response.Content.ReadAsStringAsync().Result;
            return GetJsonFromString(result);
        }
    }
}