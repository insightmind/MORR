using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Modules.Clipboard;
using MORR.Modules.Clipboard.Producers;
using MORR.Shared.Hook;
using SharedTest.TestHelpers.INativeHook;

namespace ClipboardTest
{
    [TestClass]
    public class ClipboardModuleTest
    {
        private ClipboardCopyEventProducer clipboardCopyEventProducer;
        private ClipboardCutEventProducer clipboardCutEventProducer;
        private ClipboardModule clipboardModule;
        private ClipboardPasteEventProducer clipboardPasteEventProducer;

        private CompositionContainer container;
        private HookNativeMethodsMock hookNativeMethods;

        [TestInitialize]
        public void BeforeTest()
        {
            clipboardModule = new ClipboardModule();
            clipboardCopyEventProducer = new ClipboardCopyEventProducer();
            clipboardCutEventProducer = new ClipboardCutEventProducer();
            clipboardPasteEventProducer = new ClipboardPasteEventProducer();

            container = new CompositionContainer();
            container.ComposeExportedValue(clipboardCopyEventProducer);
            container.ComposeExportedValue(clipboardCutEventProducer);
            container.ComposeExportedValue(clipboardPasteEventProducer);
            container.ComposeParts(clipboardModule);

            hookNativeMethods = new HookNativeMethodsMock();
            hookNativeMethods.Initialize();
        }

        [TestMethod]
        public void TestClipboardModule_Activate()
        {
            /* PRECONDITIONS */
            Debug.Assert(clipboardModule != null);
            Debug.Assert(clipboardCopyEventProducer != null);
            Debug.Assert(clipboardCutEventProducer != null);
            Debug.Assert(clipboardPasteEventProducer != null);
            Debug.Assert(hookNativeMethods != null);

            /* GIVEN */

            /* WHEN */
            clipboardModule.Initialize(true);
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_CLIPBOARDUPDATE);
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_PASTE);
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_CUT);
            hookNativeMethods.AllowLibraryLoad();
            clipboardModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(clipboardModule.IsActive);
            clipboardModule.IsActive = false;
        }

        [TestMethod]
        public void TestWindowManagementModule_Deactivate()
        {
            /* PRECONDITIONS */
            Debug.Assert(clipboardModule != null);
            Debug.Assert(clipboardCopyEventProducer != null);
            Debug.Assert(clipboardCutEventProducer != null);
            Debug.Assert(clipboardPasteEventProducer != null);

            /* GIVEN */

            /* WHEN */
            clipboardModule.Initialize(true);
            clipboardModule.IsActive = false;

            /* THEN */
            Assert.IsFalse(clipboardModule.IsActive);
        }

        [TestMethod]
        public void TestWindowManagementModule_InitializedFalse_ChannelClosed()
        {
            /* PRECONDITIONS */
            Debug.Assert(clipboardModule != null);
            Debug.Assert(clipboardCopyEventProducer != null);
            Debug.Assert(clipboardCutEventProducer != null);
            Debug.Assert(clipboardPasteEventProducer != null);

            /* GIVEN */

            /* WHEN */
            clipboardModule.Initialize(false);

            /* THEN */
            Assert.IsTrue(clipboardCopyEventProducer.IsClosed);
            Assert.IsTrue(clipboardCutEventProducer.IsClosed);
            Assert.IsTrue(clipboardPasteEventProducer.IsClosed);
        }

        [TestMethod]
        public void TestWindowManagementModule_InitializedTrue_ChannelOpened()
        {
            /* PRECONDITIONS */
            Debug.Assert(clipboardModule != null);
            Debug.Assert(clipboardCopyEventProducer != null);
            Debug.Assert(clipboardCutEventProducer != null);
            Debug.Assert(clipboardPasteEventProducer != null);

            /* GIVEN */

            /* WHEN */
            clipboardModule.Initialize(true);

            /* THEN */
            Assert.IsFalse(clipboardCopyEventProducer.IsClosed);
            Assert.IsFalse(clipboardCutEventProducer.IsClosed);
            Assert.IsFalse(clipboardPasteEventProducer.IsClosed);
        }
    }
}