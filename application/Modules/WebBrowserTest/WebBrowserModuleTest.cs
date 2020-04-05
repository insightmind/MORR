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
using MORR.Modules.WebBrowser.Events;
using MORR.Modules.WebBrowser.Producers;

namespace WebBrowserTest
{
    [TestClass]
    public class WebBrowserModuleTest
    {
        private static readonly WebBrowserModuleConfiguration config = new TestWebBrowserModuleConfiguration();
        private static HttpClient testClient;
        private Mock<ButtonClickEventProducer> buttonClickEventProducer;
        private Mock<CloseTabEventProducer> closeTabEventProducer;
        private CompositionContainer container;
        private Mock<FileDownloadEventProducer> fileDownloadEventProducer;
        private Mock<HoverEventProducer> hoverEventProducer;
        private JsonElement? lastJson;
        private Mock<NavigationEventProducer> navigationEventProducer;
        private Mock<OpenTabEventProducer> openTabEventProducer;
        private Mock<SwitchTabEventProducer> switchTabEventProducer;
        private WebBrowserModuleConfiguration testWebBrowserModuleConfiguration;
        private Mock<TextInputEventProducer> textInputEventProducer;
        private Mock<TextSelectionEventProducer> textSelectionEventProducer;
        private WebBrowserModule webBrowserModule;

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
            lastJson = null;
            webBrowserModule = new WebBrowserModule();
            buttonClickEventProducer = new Mock<ButtonClickEventProducer>();
            buttonClickEventProducer.Setup(p => p.Notify(It.IsAny<JsonElement>()))
                                    .Callback<JsonElement>(r => lastJson = r);
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
        //verify that connect-request is correctly handled
        public async Task SendConnectionRequest()
        {
            // Preconditions
            webBrowserModule.Initialize(true);
            webBrowserModule.IsActive = true;

            /* WHEN */
            var result = await SendHTTPMessage(new { Request = "Connect" });

            /* THEN */
            Assert.AreEqual(StringConstants.okString, result.GetProperty("response").GetString());
            Assert.AreEqual(StringConstants.appIdentifier, result.GetProperty("application").GetString());
        }

        [TestMethod]
        //verify that an invalid request is correctly answered
        public async Task SendInvalidRequest()
        {
            // Preconditions
            webBrowserModule.Initialize(true);
            webBrowserModule.IsActive = true;

            /* WHEN */
            var result = await SendHTTPMessage(new { Request = "HelloItsMe" });

            /* THEN */
            Assert.AreEqual(StringConstants.invalidString, result.GetProperty("response").GetString());
            Assert.AreEqual(StringConstants.appIdentifier, result.GetProperty("application").GetString());
        }

        [TestMethod]
        //verify that config-request is correctly handled
        public async Task SendConfigRequest()
        {
            // Preconditions
            webBrowserModule.Initialize(true);
            webBrowserModule.IsActive = true;

            /* WHEN */
            var result = await SendHTTPMessage(new { Request = "Config" });

            /* THEN */
            Assert.AreEqual(StringConstants.okString, result.GetProperty("response").GetString());
            Assert.AreEqual(StringConstants.appIdentifier, result.GetProperty("application").GetString());
            Assert.IsTrue(result.TryGetProperty("config", out var config));
            //Sending a config from MORR to the webextension is only added as a stub (unused),
            //so we only verify that any config string is sent.
            Assert.AreNotEqual(0, config.ToString().Length);
        }

        [TestMethod]
        //verify that senddata-request is correctly handled
        public async Task SendData()
        {
            // Preconditions
            webBrowserModule.Initialize(true);
            webBrowserModule.IsActive = true;
            var data = new
            {
                buttonTitle = "SomeText",
                buttonHref = "https://sample.com/redirect",
                tabID = 5,
                url = "https://sample.com",
                timeStamp = new DateTime(2015, 5, 6, 7, 8, 9, 512, DateTimeKind.Utc),
                type = "BUTTONCLICK"
            };
            /* WHEN */
            var result = await SendHTTPMessage(new
            {
                Request = "SendData",
                Data = data
            });

            /* THEN */
            Assert.AreEqual(StringConstants.okString, result.GetProperty("response").GetString());
            Assert.AreEqual(StringConstants.appIdentifier, result.GetProperty("application").GetString());

            buttonClickEventProducer.Verify(mock => mock.Notify(It.IsAny<JsonElement>()), Times.Once);
            Assert.IsNotNull(lastJson);
            var parsedEvent = new ButtonClickEvent();
            parsedEvent.Deserialize((JsonElement) lastJson);

            Assert.AreEqual(data.timeStamp.ToUniversalTime(), parsedEvent.Timestamp.ToUniversalTime());
            Assert.AreEqual(data.buttonTitle, parsedEvent.Button);
            Assert.AreEqual(data.buttonHref, parsedEvent.Href);
            Assert.AreEqual(data.tabID, parsedEvent.TabID);
            Assert.AreEqual(new Uri(data.url), parsedEvent.CurrentURL);
        }

        [TestMethod]
        //verify that senddata-request is correctly handled of no recording is active
        public async Task SendDataNotRecording()
        {
            // Preconditions
            webBrowserModule.Initialize(true);
            var data = new
            {
                buttonTitle = "SomeText",
                buttonHref = "https://sample.com/redirect",
                tabID = 5,
                url = "https://sample.com",
                timeStamp = new DateTime(2015, 5, 6, 7, 8, 9, 512),
                type = "BUTTONCLICK"
            };
            /* WHEN */
            var result = await SendHTTPMessage(new
            {
                Request = "SendData",
                Data = data
            });

            /* THEN */
            Assert.AreEqual(StringConstants.stopString, result.GetProperty("response").GetString());
            Assert.AreEqual(StringConstants.appIdentifier, result.GetProperty("application").GetString());
        }

        [TestMethod]
        //verify that start-request is correctly answered
        public async Task IssueStart()
        {
            // Preconditions
            webBrowserModule.Initialize(true);

            /* WHEN */
            await ChangeModuleActiveState(true);
            var result = await SendHTTPMessage(new { Request = "Start" });

            /* THEN */
            Assert.AreEqual(StringConstants.startString, result.GetProperty("response").GetString());
            Assert.AreEqual(StringConstants.appIdentifier, result.GetProperty("application").GetString());
        }


        [TestMethod]
        //verify that start-request is not answered until module is started
        public async Task QueueStart()
        {
            // Preconditions
            webBrowserModule.Initialize(true);

            /* WHEN */
            Task result = SendHTTPMessage(new { Request = "Start" });

            /* THEN */
            //wait for two seconds since waiting for an infinite time is not practical
            if (await Task.WhenAny(result, Task.Delay(2000)) == result)
            {
                Assert.Fail("Start request should not receive response until module is active.");
            }

            webBrowserModule.IsActive = true;
            await result;
        }

        [TestMethod]
        //verify that stop-request is correctly answered
        public async Task IssueStop()
        {
            // Preconditions
            webBrowserModule.Initialize(true);
            webBrowserModule.IsActive = false;

            /* WHEN */
            await ChangeModuleActiveState(false);
            var result = await SendHTTPMessage(new { Request = "WAITSTOP" });

            /* THEN */
            Assert.AreEqual(StringConstants.stopString, result.GetProperty("response").GetString());
            Assert.AreEqual(StringConstants.appIdentifier, result.GetProperty("application").GetString());
        }

        [TestMethod]
        //verify that stop-request is not answered until module is stopped
        public async Task QueueStop()
        {
            // Preconditions
            webBrowserModule.Initialize(true);
            webBrowserModule.IsActive = true;

            /* WHEN */
            Task result = SendHTTPMessage(new { Request = "WAITSTOP" });

            /* THEN */
            //wait for two seconds since waiting for an infinite time is not practical
            if (await Task.WhenAny(result, Task.Delay(2000)) == result)
            {
                Assert.Fail("Stop request should not receive response until module is inactive.");
            }

            webBrowserModule.IsActive = false;
            await result;
        }

        [TestMethod]
        public async Task StopListeningSuccess()
        {
            // Preconditions
            webBrowserModule.Initialize(true);

            /* THEN */
            var result = await SendHTTPMessage(new { Request = "STOPLISTENING" });

            /* THEN */
            Assert.AreEqual(StringConstants.okString, result.GetProperty("response").GetString());
            Assert.AreEqual(StringConstants.appIdentifier, result.GetProperty("application").GetString());
        }

        [TestMethod]
        public async Task StopListeningDenied()
        {
            // Preconditions
            webBrowserModule.Initialize(true);
            webBrowserModule.IsActive = true;
            /* THEN */
            var result = await SendHTTPMessage(new { Request = "STOPLISTENING" });

            /* THEN */
            Assert.AreEqual(StringConstants.requestDenied, result.GetProperty("response").GetString());
            Assert.AreEqual(StringConstants.appIdentifier, result.GetProperty("application").GetString());
            Assert.IsTrue((webBrowserModule.IsActive));
        }

        private async Task ChangeModuleActiveState(bool active)
        {
            await Task.Run(() =>
            {
                Task.Delay(200);
                webBrowserModule.IsActive = active;
            });
        }

        private static JsonElement GetJsonFromString(string data)
        {
            return JsonDocument.Parse(data).RootElement;
        }

        private static void InitializeHTTPClient(string UrlSuffix)
        {
            testClient = new HttpClient();
            var requestUri = new Uri("http://localhost:" + UrlSuffix);
            testClient.BaseAddress = requestUri;
            testClient.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json")); //ACCEPT header
        }

        private async Task<JsonElement> SendHTTPMessage(object data)
        {
            var requestUri = new Uri("http://localhost:" + config.UrlSuffix);
            var request = new HttpRequestMessage(HttpMethod.Post, "relativeAddress");
            request.Content = new StringContent(JsonSerializer.Serialize(data),
                                                Encoding.UTF8,
                                                "application/json"); //CONTENT-TYPE header

            var response = await testClient.PostAsync(requestUri, request.Content);
            var result = response.Content.ReadAsStringAsync().Result;
            return GetJsonFromString(result);
        }

        private struct StringConstants
        {
            public const string appIdentifier = "MORR";
            public const string okString = "Ok";
            public const string startString = "Start";
            public const string stopString = "Stop";
            public const string invalidString = "Invalid Request";
            public const string requestDenied = "Denied";
        }

        private class TestWebBrowserModuleConfiguration : WebBrowserModuleConfiguration
        {
            public TestWebBrowserModuleConfiguration()
            {
                UrlSuffix = "60024/";
            }
        }
    }
}