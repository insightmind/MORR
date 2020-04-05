using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.Clipboard;
using MORR.Modules.Clipboard.Producers;
using MORR.Shared.Hook;
using SharedTest.TestHelpers.INativeHook;

namespace ClipboardTest
{
    [TestClass]
    public class ClipboardPasteEventProducerTest
    {
        protected const int MaxWaitTime = 500;
        private ClipboardCopyEventProducer clipboardCopyEventProducer;
        private ClipboardCutEventProducer clipboardCutEventProducer;
        private ClipboardModule clipboardModule;
        private ClipboardPasteEventProducer clipboardPasteEventProducer;

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

            nativeClipboard = new NativeClipboardMock();

            hookNativeMethods = new HookNativeMethodsMock();
            hookNativeMethods.Initialize();
        }

        [TestCleanup]
        public void AfterTest()
        {
            clipboardModule = null;
            clipboardCopyEventProducer = null;
            clipboardPasteEventProducer = null;
            clipboardCutEventProducer = null;
            nativeClipboard = null;
            container.Dispose();
            container = null;
            hookNativeMethods = null;
        }

        [TestMethod]
        public void ClipboardPasteEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(clipboardModule != null);
            Debug.Assert(clipboardPasteEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);
            Debug.Assert(nativeClipboard != null);
            Debug.Assert(nativeClipboard.Mock != null);

            /* GIVEN */
            var callback = GetCallback();

            nativeClipboard.GetText();

            using var consumedEvent = new CountdownEvent(1);

            using var didStartConsumingEvent = new ManualResetEvent(false);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await(
                    clipboardPasteEventProducer.GetEvents(), didStartConsumingEvent))
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
            callback(new GlobalHook.HookMessage
                         { Type = (uint) GlobalHook.MessageType.WM_PASTE });

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime), "Did not find all matching clipboard paste event in time.");

            //total shut down and resources release
            clipboardPasteEventProducer.StopCapture();
            clipboardModule.Initialize(false);
        }

        private GlobalHook.CppGetMessageCallback GetCallback()
        {
            GlobalHook.CppGetMessageCallback callback = null;

            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_CLIPBOARDUPDATE);
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_PASTE);
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_CUT);

            hookNativeMethods.AllowLibraryLoad();
            var callbackReceivedEvent = new AutoResetEvent(false);

            hookNativeMethods.Mock
                             .Setup(
                                 hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()))?
                             .Callback((GlobalHook.CppGetMessageCallback cppCallback, bool isBlocking) =>
                             {
                                 callback = cppCallback;
                                 callbackReceivedEvent.Set();
                             });
            //here the SetHook() method is called!
            clipboardModule.Initialize(true);
            clipboardPasteEventProducer.StartCapture(nativeClipboard.Mock.Object);

            //wait for the hookNativeMethodsMock.Mock.Callback is called!
            Assert.IsTrue(callbackReceivedEvent.WaitOne(MaxWaitTime), "Did not receive callback in time!");
            callbackReceivedEvent.Dispose();
            Assert.IsNotNull(callback, "Callback received however unexpectedly null!");
            return callback;
        }

        public static T Await<T>(T awaitedObject, ManualResetEvent expectation)
        {
            expectation.Set();
            return awaitedObject;
        }
    }
}