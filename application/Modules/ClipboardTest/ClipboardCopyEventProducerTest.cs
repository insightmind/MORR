using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Modules.Clipboard;
using MORR.Modules.Clipboard.Producers;
using MORR.Shared.Hook;
using SharedTest.TestHelpers.INativeHook;

namespace ClipboardTest
{
    [TestClass]
    public class ClipboardCopyEventProducerTest
    {
        protected const int MaxWaitTime = 500;
        private ClipboardCopyEventProducer clipboardCopyEventProducer;
        private ClipboardCutEventProducer clipboardCutEventProducer;
        private ClipboardModule clipboardModule;
        private ClipboardPasteEventProducer clipboardPasteEventProducer;
        private ClipboardWindowMessageSinkMock clipWinMsgSinkMock;

        private CompositionContainer container;
        private HookNativeMethodsMock hookNativeMethods;
        private NativeClipboardMock nativeClipboard;

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

            clipWinMsgSinkMock = new ClipboardWindowMessageSinkMock();
            hookNativeMethods = new HookNativeMethodsMock();
            hookNativeMethods.Initialize();
            nativeClipboard = new NativeClipboardMock();
        }

        [TestCleanup]
        public void AfterTest()
        {
            clipboardModule = null;
            clipboardCopyEventProducer = null;
            clipboardPasteEventProducer = null;
            clipboardCutEventProducer = null;
            nativeClipboard = null;
            clipWinMsgSinkMock = null;
            container.Dispose();
            container = null;
            hookNativeMethods = null;
        }

        [TestMethod]
        public void ClipboardCopyEventProducerWinMessageSinkTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(clipboardModule != null);
            Debug.Assert(clipboardCopyEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(nativeClipboard != null);
            Debug.Assert(clipWinMsgSinkMock != null);
            Debug.Assert(clipWinMsgSinkMock.Mock != null);
            /* GIVEN */
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_CLIPBOARDUPDATE);
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_PASTE);
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_CUT);

            hookNativeMethods.AllowLibraryLoad();

            clipWinMsgSinkMock.Dispose();
            nativeClipboard.GetText();

            clipboardModule.Initialize(true);
            clipboardCopyEventProducer.StartCapture(clipWinMsgSinkMock.Mock.Object, nativeClipboard.Mock.Object);


            using var consumedEvent = new CountdownEvent(1);

            using var didStartConsumingEvent = new ManualResetEvent(false);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await(
                    clipboardCopyEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!@event.ClipboardText.Equals("ClipboardText"))
                    {
                        continue;
                    }

                    consumedEvent.Signal();
                }
            });

            thread.Start();

            Assert.IsTrue(didStartConsumingEvent.WaitOne(MaxWaitTime));
            clipWinMsgSinkMock.ClipboardUpdateCopy();

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime), "Did not find all matching clipboard copy event in time.");
            clipboardCopyEventProducer.StopCapture();
            clipboardModule.Initialize(false);
        }

        public static T Await<T>(T awaitedObject, ManualResetEvent expectation)
        {
            expectation.Set();
            return awaitedObject;
        }
    }
}