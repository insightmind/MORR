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
    public class ClipboardCutEventProducerTest
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
            clipWinMsgSinkMock = null;
            container.Dispose();
            container = null;
            hookNativeMethods = null;
        }

        [TestMethod]
        public void ClipboardCutEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(clipboardModule != null);
            Debug.Assert(clipboardCutEventProducer != null);
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
                    clipboardCutEventProducer.GetEvents(), didStartConsumingEvent))
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
                         { Type = (uint) GlobalHook.MessageType.WM_CUT });

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime), "Did not find all matching clipboard paste event in time.");

            //total shut down and resources release
            clipboardCutEventProducer.StopCapture();
            clipboardModule.Initialize(false);
        }

        [TestMethod]
        public void ClipboardCutEventProducerWindMessageSinkTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(clipboardModule != null);
            Debug.Assert(clipboardCutEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(nativeClipboard != null);
            Debug.Assert(clipWinMsgSinkMock != null);
            Debug.Assert(clipWinMsgSinkMock.Mock != null);
            Debug.Assert(nativeClipboard.Mock != null);

            /* GIVEN */
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_CLIPBOARDUPDATE);
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_PASTE);
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_CUT);

            hookNativeMethods.AllowLibraryLoad();

            clipWinMsgSinkMock.Dispose();
            nativeClipboard.GetText();

            clipboardModule.Initialize(true);
            clipboardCutEventProducer.StartCapture(clipWinMsgSinkMock.Mock.Object, nativeClipboard.Mock.Object);


            using var consumedEvent = new CountdownEvent(1);

            using var didStartConsumingEvent = new ManualResetEvent(false);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await(
                    clipboardCutEventProducer.GetEvents(), didStartConsumingEvent))
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
            clipWinMsgSinkMock.ClipboardUpdateCut();

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime), "Did not find all matching clipboard cut event in time.");
            clipboardCutEventProducer.StopCapture();
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
            clipboardCutEventProducer.StartCapture(clipWinMsgSinkMock.Mock.Object, nativeClipboard.Mock.Object);

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