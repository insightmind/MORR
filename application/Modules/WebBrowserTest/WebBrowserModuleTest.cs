using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.WebBrowser;
using MORR.Modules.WebBrowser.Producers;

namespace WebBrowserTest
{
    [TestClass]
    public class WebBrowserModuleTest
    {
        private WebBrowserModule webBrowserModule;
        private Mock<ButtonClickEventProducer> buttonClickEventProducer;
        private Mock<CloseTabEventProducer> closeTabEventProducer;
        private Mock<FileDownloadEventProducer> fileDownloadEventProducer;
        private Mock<HoverEventProducer> hoverEventProducer;
        private Mock<NavigationEventProducer> navigationEventProducer;
        private Mock<OpenTabEventProducer> openTabEventProducer;
        private Mock<SwitchTabEventProducer> switchTabEventProducer;
        private Mock<TextInputEventProducer> textInputEventProducer;
        private Mock<TextSelectionEventProducer> textSelectionEventProducer;
        private WebBrowserModuleConfiguration testWebBrowserModuleConfiguration;
        private CompositionContainer container;

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

        private class TestWebBrowserModuleConfiguration : WebBrowserModuleConfiguration
        {
            public new string UrlSuffix { get; set; } = "60024/";
        }
    }
}