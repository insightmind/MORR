using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.WindowManagement;
using MORR.Modules.WindowManagement.Events;
using MORR.Modules.WindowManagement.Producers;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;
using SharedTest.Hook;
using SharedTest.TestHelpers.INativeHook;

namespace WindowManagementTest
{
    [TestClass]
    public class WindowManagementModuleTest
    {
        protected const int MaxWaitTime = 500;

        private CompositionContainer container;
        private WindowFocusEventProducer windowFocusEventProducer;
        private WindowManagementModule windowManagementModule;
        private WindowMovementEventProducer windowMovementEventProducer;
        private WindowResizingEventProducer windowResizingEventProducer;
        private WindowStateChangedEventProducer windowStateChangedEventProducer;
        private HookNativeMethodsMock hookNativeMethods;

        private readonly GlobalHook.MessageType[] windowEventListenedMessagesTypes = 
        {
            GlobalHook.MessageType.WM_ACTIVATE,
            GlobalHook.MessageType.WM_ENTERSIZEMOVE,
            GlobalHook.MessageType.WM_EXITSIZEMOVE,
            GlobalHook.MessageType.WM_SIZE
        };


        [TestInitialize]
        public void BeforeTest()
        {
            windowManagementModule = new WindowManagementModule();
            windowFocusEventProducer = new WindowFocusEventProducer();
            windowMovementEventProducer = new WindowMovementEventProducer();
            windowResizingEventProducer = new WindowResizingEventProducer();
            windowStateChangedEventProducer = new WindowStateChangedEventProducer();

            container = new CompositionContainer();
            container.ComposeExportedValue(windowFocusEventProducer);
            container.ComposeExportedValue(windowMovementEventProducer);
            container.ComposeExportedValue(windowResizingEventProducer);
            container.ComposeExportedValue(windowStateChangedEventProducer);
            container.ComposeParts(windowManagementModule);


            hookNativeMethods = new HookNativeMethodsMock();
            hookNativeMethods.Initialize();
        }

        [TestMethod]
        public void TestWindowManagementModule_Activate()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowFocusEventProducer != null);
            Debug.Assert(windowMovementEventProducer != null);
            Debug.Assert(windowResizingEventProducer != null);
            Debug.Assert(windowStateChangedEventProducer != null);
            Debug.Assert(hookNativeMethods != null);

            /* GIVEN */

            /* WHEN */
            windowManagementModule.Initialize(true);
            foreach (GlobalHook.MessageType messageType in windowEventListenedMessagesTypes)
            {
                hookNativeMethods.AllowMessageTypeRegistry(messageType);
            }
            hookNativeMethods.AllowLibraryLoad();
            windowManagementModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(windowManagementModule.IsActive);
        }

        [TestMethod]
        public void TestWindowManagementModule_Deactivate()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowFocusEventProducer != null);
            Debug.Assert(windowMovementEventProducer != null);
            Debug.Assert(windowResizingEventProducer != null);
            Debug.Assert(windowStateChangedEventProducer != null);

            /* GIVEN */

            /* WHEN */
            windowManagementModule.Initialize(true);
            windowManagementModule.IsActive = false;

            /* THEN */
            Assert.IsFalse(windowManagementModule.IsActive);
        }
        [TestMethod]
        public void TestWindowManagementModule_InitializedFalse_ChannelClosed()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowFocusEventProducer != null);
            Debug.Assert(windowMovementEventProducer != null);
            Debug.Assert(windowResizingEventProducer != null);
            Debug.Assert(windowStateChangedEventProducer != null);

            /* GIVEN */

            /* WHEN */
            windowManagementModule.Initialize(false);

            /* THEN */
            Assert.IsTrue(windowFocusEventProducer.IsClosed);
            Assert.IsTrue(windowMovementEventProducer.IsClosed);
            Assert.IsTrue(windowResizingEventProducer.IsClosed);
            Assert.IsTrue(windowStateChangedEventProducer.IsClosed);
        }

        [TestMethod]
        public void TestWindowManagementModule_InitializedTrue_ChannelOpened()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowFocusEventProducer != null);
            Debug.Assert(windowMovementEventProducer != null);
            Debug.Assert(windowResizingEventProducer != null);
            Debug.Assert(windowStateChangedEventProducer != null);

            /* GIVEN */

            /* WHEN */
            windowManagementModule.Initialize(true);

            /* THEN */
            Assert.IsFalse(windowFocusEventProducer.IsClosed);
            Assert.IsFalse(windowMovementEventProducer.IsClosed);
            Assert.IsFalse(windowResizingEventProducer.IsClosed);
            Assert.IsFalse(windowStateChangedEventProducer.IsClosed);
        }

        [TestMethod]
        public async Task WindowFocusEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowFocusEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            const int WA_ACTIVE = 1;

            var callbackReceivedEvent = new AutoResetEvent(false);
            GlobalHook.CppGetMessageCallback callback = null;

            foreach (GlobalHook.MessageType messageType in windowEventListenedMessagesTypes)
            {
                hookNativeMethods.AllowMessageTypeRegistry(messageType);
            }
            hookNativeMethods.AllowLibraryLoad();
            hookNativeMethods.Mock
                                 .Setup(hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()))?
                                 .Callback((GlobalHook.CppGetMessageCallback cppCallback, bool isBlocking) =>
                                 {
                                     callback = cppCallback;
                                     callbackReceivedEvent.Set();
                                 });
            /* WHEN */
            windowManagementModule.Initialize(true);
            windowManagementModule.IsActive = true;
            Assert.IsTrue(callbackReceivedEvent.WaitOne(MaxWaitTime), "Did not receive callback in time!");
            Assert.IsNotNull(callback, "Callback received however unexpectedly null!");

            var consumedEvent = new ManualResetEvent(false);

            var task = new Task(() => findMatch(windowFocusEventProducer, consumedEvent, (@event) => @event.Title.Equals("sampleFocusTitle")));
            task.Start();
            callback(new GlobalHook.HookMessage { Type = (uint)windowEventListenedMessagesTypes[0], wParam = (IntPtr)WA_ACTIVE, Data = new[] { 1 } });
            Assert.IsTrue(consumedEvent.WaitOne(MaxWaitTime), "Did not find a matching window event in time.");
        }

        [TestMethod]
        public async Task WindowMovementEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowMovementEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            var callbackReceivedEvent = new AutoResetEvent(false);
            GlobalHook.CppGetMessageCallback callback = null;

            foreach (GlobalHook.MessageType messageType in windowEventListenedMessagesTypes)
            {
                hookNativeMethods.AllowMessageTypeRegistry(messageType);
            }
            hookNativeMethods.AllowLibraryLoad();
            hookNativeMethods.Mock
                             .Setup(hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()))?
                             .Callback((GlobalHook.CppGetMessageCallback cppCallback, bool isBlocking) =>
                             {
                                 callback = cppCallback;
                                 callbackReceivedEvent.Set();
                             });
            /* WHEN */
            windowManagementModule.Initialize(true);
            windowManagementModule.IsActive = true;
            Assert.IsTrue(callbackReceivedEvent.WaitOne(MaxWaitTime), "Did not receive callback in time!");
            Assert.IsNotNull(callback, "Callback received however unexpectedly null!");

            var consumedEvent = new ManualResetEvent(false);

            var task = new Task(() => findMatch(windowMovementEventProducer, consumedEvent, (@event) => @event.Title.Equals("sampleMovementTitle")));
            task.Start();

            callback(new GlobalHook.HookMessage { Type = (uint)windowEventListenedMessagesTypes[1], Hwnd = (IntPtr)1 });
            callback(new GlobalHook.HookMessage { Type = (uint)windowEventListenedMessagesTypes[2], Data = new[] { 1 } });
            Assert.IsTrue(consumedEvent.WaitOne(MaxWaitTime), "Did not find a matching window event in time.");
        }

        [TestMethod]
        public async Task WindowResizingEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowResizingEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            var callbackReceivedEvent = new AutoResetEvent(false);
            GlobalHook.CppGetMessageCallback callback = null;

            foreach (GlobalHook.MessageType messageType in windowEventListenedMessagesTypes)
            {
                hookNativeMethods.AllowMessageTypeRegistry(messageType);
            }
            hookNativeMethods.AllowLibraryLoad();
            hookNativeMethods.Mock
                             .Setup(hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()))?
                             .Callback((GlobalHook.CppGetMessageCallback cppCallback, bool isBlocking) =>
                             {
                                 callback = cppCallback;
                                 callbackReceivedEvent.Set();
                             });
            /* WHEN */
            windowManagementModule.Initialize(true);
            windowManagementModule.IsActive = true;
            Assert.IsTrue(callbackReceivedEvent.WaitOne(MaxWaitTime), "Did not receive callback in time!");
            Assert.IsNotNull(callback, "Callback received however unexpectedly null!");

            var consumedEvent = new ManualResetEvent(false);

            var task = new Task(() => findMatch(windowResizingEventProducer, consumedEvent, (@event) => @event.Title.Equals("sampleResizingTitle")));
            task.Start();

            callback(new GlobalHook.HookMessage { Type = (uint)windowEventListenedMessagesTypes[1], Hwnd = (IntPtr)1 });
            callback(new GlobalHook.HookMessage { Type = (uint)windowEventListenedMessagesTypes[2], Data = new[] { 2 } });
            Assert.IsTrue(consumedEvent.WaitOne(MaxWaitTime), "Did not find a matching window event in time.");
        }

        [TestMethod]
        public async Task WindowStateChangedEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowStateChangedEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            var callbackReceivedEvent = new AutoResetEvent(false);
            GlobalHook.CppGetMessageCallback callback = null;

            foreach (GlobalHook.MessageType messageType in windowEventListenedMessagesTypes)
            {
                hookNativeMethods.AllowMessageTypeRegistry(messageType);
            }
            hookNativeMethods.AllowLibraryLoad();
            hookNativeMethods.Mock
                             .Setup(hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()))?
                             .Callback((GlobalHook.CppGetMessageCallback cppCallback, bool isBlocking) =>
                             {
                                 callback = cppCallback;
                                 callbackReceivedEvent.Set();
                             });
            /* WHEN */
            windowManagementModule.Initialize(true);
            windowManagementModule.IsActive = true;
            Assert.IsTrue(callbackReceivedEvent.WaitOne(MaxWaitTime), "Did not receive callback in time!");
            Assert.IsNotNull(callback, "Callback received however unexpectedly null!");

            var consumedEvent = new ManualResetEvent(false);

            var task = new Task(() => findMatch(windowStateChangedEventProducer, consumedEvent, (@event) => @event.Title.Equals("sampleStateChangedTitle")));
            task.Start();

            callback(new GlobalHook.HookMessage { Type = (uint)windowEventListenedMessagesTypes[1], Hwnd = (IntPtr)1 });
            callback(new GlobalHook.HookMessage { Type = (uint)windowEventListenedMessagesTypes[2], Data = new[] { 3 } });
            Assert.IsTrue(consumedEvent.WaitOne(MaxWaitTime), "Did not find a matching window event in time.");
        }

        private async void findMatch<T>(DefaultEventQueue<T> producer, ManualResetEvent reset, Func<T, bool> predicate) where T : WindowEvent
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