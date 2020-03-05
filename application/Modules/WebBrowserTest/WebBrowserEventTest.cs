using System;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Modules.WebBrowser.Events;

namespace WebBrowserTest
{
    [TestClass]
    public class WebBrowserEventTest
    {
        private readonly DateTime commonDateTime = new DateTime(2020, 03, 05, 10, 10, 51, 123);
        private const int commonTabId = 5;
        private const string commonText = "SomeText";
        private readonly Uri commonUrl = new Uri("https://sample.com/");
        private const string serializedEvent = "{\"buttonTitle\":\"SomeText\",\"buttonHref\":\"https://sample.com/redirect\"," +
            "\"tabID\":5,\"url\":\"https://sample.com\"," +
            "\"timeStamp\":\"2020-03-05T10:10:51.1230000+01:00\",\"fileURL\":\"https://sample.com/download\",\"mimeType\":\"JSON\"," +
            "\"target\":\"SomeLabel\",\"newTabID\":7,\"text\":\"SomeInput\",\"textSelection\":\"SomeSelection\"}";

        //test if ButtonClick attributes are correctly deserialized
        [TestMethod]
        public void DeserializeButtonClickEvent()
        {
            /* GIVEN */
            string eventJson = "{\"buttonTitle\":\"SomeText\",\"buttonHref\":\"https://sample.com/redirect\",\"tabID\":5,\"url\":\"https://sample.com\",\"timeStamp\":\"2020-03-05T10:10:51.1230000+01:00\"}";
            var deserialized = new ButtonClickEvent();

            /* WHEN */
            deserialized.Deserialize(eventJson);

            /* THEN */
            TestCommonAttributes(deserialized);
            Assert.AreEqual(commonText, deserialized.Button);
            Assert.AreEqual("https://sample.com/redirect", deserialized.Href);
        }

        //test if CloseTabEvent attributes are correctly deserialized
        [TestMethod]
        public void DeserializeCloseTabEvent()
        {
            /* GIVEN */
            var deserialized = new CloseTabEvent();

            /* WHEN */
            deserialized.Deserialize(serializedEvent);

            /* THEN */
            TestCommonAttributes(deserialized);
        }

        //test if FileDownloadEvent attributes are correctly deserialized
        [TestMethod]
        public void DeserializeFileDownloadEvent()
        {
            /* GIVEN */
            var deserialized = new FileDownloadEvent();

            /* WHEN */
            deserialized.Deserialize(serializedEvent);

            /* THEN */
            TestCommonAttributes(deserialized);
            Assert.AreEqual(new Uri("https://sample.com/download"), deserialized.FileURL);
            Assert.AreEqual("JSON", deserialized.MIMEType);
        }

        //test if HoverEvent attributes are correctly deserialized
        [TestMethod]
        public void DeserializeHoverEvent()
        {
            /* GIVEN */
            var deserialized = new HoverEvent();

            /* WHEN */
            deserialized.Deserialize(serializedEvent);

            /* THEN */
            TestCommonAttributes(deserialized);
            Assert.AreEqual("SomeLabel", deserialized.HoveredElement);
        }

        //test if NavigationEvent attributes are correctly deserialized
        [TestMethod]
        public void DeserializeNavigationEvent()
        {
            /* GIVEN */
            var deserialized = new NavigationEvent();

            /* WHEN */
            deserialized.Deserialize(serializedEvent);

            /* THEN */
            TestCommonAttributes(deserialized);
        }

        //test if OpenTabEvent attributes are correctly deserialized
        [TestMethod]
        public void DeserializeOpenTabEvent()
        {
            /* GIVEN */
            var deserialized = new OpenTabEvent();

            /* WHEN */
            deserialized.Deserialize(serializedEvent);

            /* THEN */
            TestCommonAttributes(deserialized);
        }

        //test if SwitchTabEvent attributes are correctly deserialized
        [TestMethod]
        public void DeserializeSwitchTabEvent()
        {
            /* GIVEN */
            var deserialized = new SwitchTabEvent();

            /* WHEN */
            deserialized.Deserialize(serializedEvent);

            /* THEN */
            TestCommonAttributes(deserialized);
            Assert.AreEqual(7, deserialized.NewTabID);
        }

        //test if TextInputEvent attributes are correctly deserialized
        [TestMethod]
        public void DeserializeTextInputEvent()
        {
            /* GIVEN */
            var deserialized = new TextInputEvent();

            /* WHEN */
            deserialized.Deserialize(serializedEvent);

            /* THEN */
            TestCommonAttributes(deserialized);
            Assert.AreEqual(deserialized.InputtedText, "SomeInput");
            Assert.AreEqual("SomeLabel", deserialized.Textbox);
        }

        //test if TextSelectionEvent attributes are correctly deserialized
        [TestMethod]
        public void DeserializeTextSelectionEvent()
        {
            /* GIVEN */
            var deserialized = new TextSelectionEvent();

            /* WHEN */
            deserialized.Deserialize(serializedEvent);

            /* THEN */
            TestCommonAttributes(deserialized);
            Assert.AreEqual("SomeSelection", deserialized.SelectedText);
        }

        private void TestCommonAttributes(WebBrowserEvent deserialized)
        {
            Assert.AreEqual(commonUrl, deserialized.CurrentURL);
            Assert.AreEqual(commonDateTime, deserialized.Timestamp);
            Assert.AreEqual(commonTabId, deserialized.TabID);
        }
    }
}
