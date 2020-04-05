using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Modules.Clipboard.Events;

namespace ClipboardTest
{
    [TestClass]
    public class ClipboardEventTest
    {
        [TestMethod]
        public void TestClipboardEvent_Text()
        {
            /* GIVEN */
            var @event = new ClipboardEventImp();
            var clipboardText = "sampleText";

            /* WHEN */
            @event.ClipboardText = clipboardText;

            /* THEN */
            Assert.AreEqual(clipboardText, @event.ClipboardText);
        }

        public class ClipboardEventImp : ClipboardEvent { }
    }
}