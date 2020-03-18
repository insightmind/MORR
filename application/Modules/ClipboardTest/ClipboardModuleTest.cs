using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.Clipboard;
using MORR.Modules.Clipboard.Events;
using MORR.Modules.Clipboard.Producers;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;
using SharedTest.TestHelpers.INativeHook;

namespace ClipboardTest
{
    [TestClass]
    public class ClipboardModuleTest
    {
        protected const int MaxWaitTime = 500;
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
            hookNativeMethods.AllowLibraryLoad();
            clipboardModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(clipboardModule.IsActive);
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

        [TestMethod]
        public void ClipboardCopyEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(clipboardModule != null);
            Debug.Assert(clipboardCopyEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(ClipboardCopyEventProducer.clipboardWindowMessageSink != null);

            /* GIVEN */
            const int wparamTest = 10;

            var clipboardWindowMessageSink = ClipboardCopyEventProducer.clipboardWindowMessageSink;

            GetCallback();

            using var consumedEvent = new CountdownEvent(1);

            using var didStartConsumingEvent = new ManualResetEvent(false);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await(
                    clipboardCopyEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!@event.ClipboardText.Equals("sampleCopyText"))
                    {
                        continue;
                    }

                    consumedEvent.Signal();
                }
            });

            thread.Start();

            Assert.IsTrue(didStartConsumingEvent.WaitOne(MaxWaitTime));

            clipboardWindowMessageSink.ClipboardUpdateTestHelper(IntPtr.Zero,
                                                                 (int) GlobalHook.MessageType.WM_CLIPBOARDUPDATE,
                                                                 (IntPtr) wparamTest, IntPtr.Zero);
            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime), "Did not find all matching clipboard copy event in time.");
            clipboardModule.IsActive = false;
            clipboardModule.Initialize(false);
        }

        [TestMethod]
        public void ClipboardCutEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(clipboardModule != null);
            Debug.Assert(clipboardCutEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);
            Debug.Assert(ClipboardCutEventProducer.clipboardWindowMessageSink != null);

            /* GIVEN */
            const int wparamTest = 11;

            var clipboardWindowMessageSink = ClipboardCutEventProducer.clipboardWindowMessageSink;

            GetCallback();

            using var consumedEvent = new CountdownEvent(1);

            using var didStartConsumingEvent = new ManualResetEvent(false);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await(
                    clipboardCutEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!@event.ClipboardText.Equals("sampleCutText"))
                    {
                        continue;
                    }

                    consumedEvent.Signal();
                }
            });

            thread.Start();

            Assert.IsTrue(didStartConsumingEvent.WaitOne(MaxWaitTime));

            clipboardWindowMessageSink.ClipboardUpdateTestHelper(IntPtr.Zero,
                                                                 (int)GlobalHook.MessageType.WM_CLIPBOARDUPDATE,
                                                                 (IntPtr)wparamTest, IntPtr.Zero);
            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime), "Did not find all matching clipboard cut event in time.");
            clipboardModule.IsActive = false;
            clipboardModule.Initialize(false);
        }

        [TestMethod]
        public void ClipboardPasteEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(clipboardModule != null);
            Debug.Assert(clipboardPasteEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            const int dataParamTest = 12;
            var callback = GetCallback();

            using var consumedEvent = new CountdownEvent(1);

            using var didStartConsumingEvent = new ManualResetEvent(false);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await(
                    clipboardPasteEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!@event.ClipboardText.Equals("samplePasteText"))
                    {
                        continue;
                    }

                    consumedEvent.Signal();
                }
            });

            thread.Start();

            Assert.IsTrue(didStartConsumingEvent.WaitOne(MaxWaitTime));
            callback(new GlobalHook.HookMessage
                         { Type = (uint) GlobalHook.MessageType.WM_PASTE, Data = new[] { dataParamTest } });

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime), "Did not find all matching clipboard paste event in time.");

            //total shut down and resources release
            clipboardModule.IsActive = false;
            clipboardModule.Initialize(false);
        }

        private async void findMatch<T>(DefaultEventQueue<T> producer, ManualResetEvent reset, Func<T, bool> predicate)
            where T : ClipboardEvent
        {
            await foreach (var @event in Await(producer.GetEvents(), reset))
            {
                if (predicate.Invoke(@event))
                {
                    reset.Set();
                    break;
                }
            }
        }

        private GlobalHook.CppGetMessageCallback GetCallback()
        {
            GlobalHook.CppGetMessageCallback callback = null;

            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_CLIPBOARDUPDATE);
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_PASTE);

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
            clipboardModule.IsActive = true;

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