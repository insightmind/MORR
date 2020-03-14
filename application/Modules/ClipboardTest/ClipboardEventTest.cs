using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Modules.Clipboard.Events;
using MORR.Shared.Events;

namespace ClipboardTest
{
    [TestClass]
    public class ClipboardEventTest
    {
        public class ClipboardEventImp : ClipboardEvent { }

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
    }
}
