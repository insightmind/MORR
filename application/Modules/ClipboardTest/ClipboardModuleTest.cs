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

        private CompositionContainer container;
        private ClipboardCopyEventProducer clipboardCopyEventProducer;
        private ClipboardModule clipboardModule;
        private ClipboardCutEventProducer clipboardCutEventProducer;
        private ClipboardPasteEventProducer clipboardPasteEventProducer;
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
        public async Task ClipboardCopyEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(clipboardModule != null);
            Debug.Assert(clipboardCopyEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */

            var callbackReceivedEvent = new AutoResetEvent(false);
            GlobalHook.CppGetMessageCallback callback = null;

            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_CLIPBOARDUPDATE);
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_PASTE);
            hookNativeMethods.AllowLibraryLoad();
            hookNativeMethods.Mock
                             .Setup(hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()))?
                             .Callback((GlobalHook.CppGetMessageCallback cppCallback, bool isBlocking) =>
                             {
                                 callback = cppCallback;
                                 callbackReceivedEvent.Set();
                             });
            /* WHEN */
            clipboardModule.Initialize(true);
            clipboardModule.IsActive = true;

            Assert.IsTrue(callbackReceivedEvent.WaitOne(MaxWaitTime), "Did not receive callback in time!");
            Assert.IsNotNull(callback, "Callback received however unexpectedly null!");

            var consumedEvent = new ManualResetEvent(false);

            var task = new Task(() => findMatch(clipboardCopyEventProducer, consumedEvent, (@event) => @event.Title.Equals("sampleFocusTitle")));
            task.Start();
            callback(new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_CLIPBOARDUPDATE, wParam = , Data = new[] { 1 } });
            Assert.IsTrue(consumedEvent.WaitOne(MaxWaitTime), "Did not find a matching window event in time.");

        }

        private async void findMatch<T>(DefaultEventQueue<T> producer, ManualResetEvent reset, Func<T, bool> predicate) where T : ClipboardEvent
        {
            await foreach (var @event in producer.GetEvents())
            {
                if (predicate.Invoke(@event))
                {
                    reset.Set();
                    break;
                }
            }
        }
    }
}