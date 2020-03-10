using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.Clipboard;
using MORR.Modules.Clipboard.Producers;
using MORR.Shared.Hook;

namespace ClipboardTest
{
    [TestClass]
    public class ClipboardModuleTest
    {
        private Mock<ClipboardCopyEventProducer> clipboardCopyEventProducer;
        private Mock<ClipboardCutEventProducer> clipboardCutEventProducer;
        private ClipboardModule clipboardModule;
        private Mock<ClipboardPasteEventProducer> clipboardPasteEventProducer;
        private CompositionContainer container;

        private Mock<IHookNativeMethods> nativeHook;

        [TestInitialize]
        public void BeforeTest()
        {
            clipboardModule = new ClipboardModule();
            clipboardCopyEventProducer = new Mock<ClipboardCopyEventProducer>();
            clipboardCutEventProducer = new Mock<ClipboardCutEventProducer>();
            clipboardPasteEventProducer = new Mock<ClipboardPasteEventProducer>();
            nativeHook = new Mock<IHookNativeMethods>();

            container = new CompositionContainer();
            container.ComposeExportedValue(clipboardCopyEventProducer.Object);
            container.ComposeExportedValue(clipboardCutEventProducer.Object);
            container.ComposeExportedValue(clipboardPasteEventProducer.Object);
            container.ComposeParts(clipboardModule);

            nativeHook
                .Setup(hook => hook.Capture(It.IsAny<uint>()))?
                .Returns(true);

            nativeHook
                .Setup(mock => mock.LoadLibrary())?
                .Returns(new IntPtr(0x1)); // We just return a non null pointer

            GlobalHook.Initialize(nativeHook.Object);
        }

        [TestMethod]
        public void TestClipboardModule_Activate()
        {
            // Preconditions
            Debug.Assert(clipboardModule != null);

            /* GIVEN */

            /* WHEN */
            clipboardModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(clipboardModule.IsActive);
        }

        [TestMethod]
        public void TesClipboardModule_Deactivate()
        {
            // Preconditions
            Debug.Assert(clipboardModule != null);

            /* GIVEN */

            /* WHEN */
            clipboardModule.IsActive = false;

            /* THEN */
            Assert.IsFalse(clipboardModule.IsActive);
        }
    }
}