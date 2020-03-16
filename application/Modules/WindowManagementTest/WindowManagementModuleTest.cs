using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.WindowManagement;
using MORR.Modules.WindowManagement.Events;
using MORR.Modules.WindowManagement.Producers;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;
using SharedTest.TestHelpers.INativeHook;
using Size = System.Drawing.Size;

namespace WindowManagementTest
{
    [TestClass]
    public class WindowManagementModuleTest
    {
        protected const int MaxWaitTime = 500;

        private readonly GlobalHook.MessageType[] windowEventListenedMessagesTypes =
        {
            GlobalHook.MessageType.WM_ACTIVATE,
            GlobalHook.MessageType.WM_ENTERSIZEMOVE,
            GlobalHook.MessageType.WM_EXITSIZEMOVE,
            GlobalHook.MessageType.WM_SIZE
        };

        private CompositionContainer container;
        private HookNativeMethodsMock hookNativeMethods;
        private WindowFocusEventProducer windowFocusEventProducer;
        private WindowManagementModule windowManagementModule;
        private WindowMovementEventProducer windowMovementEventProducer;
        private WindowResizingEventProducer windowResizingEventProducer;
        private WindowStateChangedEventProducer windowStateChangedEventProducer;


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
            foreach (var messageType in windowEventListenedMessagesTypes)
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
            const int dataParamTest = 1;

            var callback = GetCallback();

            var consumedEvent = new ManualResetEvent(false);

            var task = new Task(() => findMatch(windowFocusEventProducer, consumedEvent,
                                                @event => @event.Title.Equals("sampleFocusTitle") 
                                                          && @event.ProcessName.Equals("sampleProcessName")));
            task.Start();
            callback(new GlobalHook.HookMessage
            {
                Type = (uint) windowEventListenedMessagesTypes[0], wParam = (IntPtr) WA_ACTIVE,
                Data = new[] { dataParamTest }
            });
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
            const int dataParamTest = 2;
            var callback = GetCallback();

            var consumedEvent = new ManualResetEvent(false);

            var task = new Task(() => findMatch(windowMovementEventProducer, consumedEvent, @event =>
                                                    @event.Title.Equals("sampleMovementTitle") 
                                                    && @event.ProcessName.Equals("sampleProcessName")
                                                    && @event.OldLocation.Equals(new Point(0, 0))
                                                    && @event.NewLocation.Equals(new Point(1, 1))));
            task.Start();

            callback(new GlobalHook.HookMessage
                         { Type = (uint) windowEventListenedMessagesTypes[1], Hwnd = (IntPtr) 1 });
            callback(new GlobalHook.HookMessage
                         { Type = (uint) windowEventListenedMessagesTypes[2], Data = new[] { dataParamTest } });
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
            const int dataParamTest = 3;
            var callback = GetCallback();

            var consumedEvent = new ManualResetEvent(false);

            var task = new Task(() => findMatch(windowResizingEventProducer, consumedEvent, @event =>
                                                    @event.Title.Equals("sampleResizingTitle") 
                                                    && @event.ProcessName.Equals("sampleProcessName")
                                                    && @event.OldSize.Equals(new Size(0, 0))
                                                    && @event.NewSize.Equals(new Size(1, 1))));
            task.Start();

            callback(new GlobalHook.HookMessage
                         { Type = (uint) windowEventListenedMessagesTypes[1], Hwnd = (IntPtr) 1 });
            callback(new GlobalHook.HookMessage
                         { Type = (uint) windowEventListenedMessagesTypes[2], Data = new[] { dataParamTest } });
            Assert.IsTrue(consumedEvent.WaitOne(MaxWaitTime), "Did not find a matching window event in time.");
        }

        [TestMethod]
        public async Task WindowStateChangedEventProducerCallbackTest_Restored()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowStateChangedEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            const int dataParamTest = 4;
            var callback = GetCallback();

            var consumedEvent = new ManualResetEvent(false);

            var task = new Task(() => findMatch(windowStateChangedEventProducer, consumedEvent, @event =>
                                                    @event.Title.Equals("sampleStateChangedTitle")
                                                    && @event.ProcessName.Equals("sampleProcessName")
                                                    && @event.WindowState == WindowState.Normal));
            task.Start();

            callback(new GlobalHook.HookMessage
                         { Type = (uint) windowEventListenedMessagesTypes[1], Hwnd = (IntPtr) 1 });
            callback(new GlobalHook.HookMessage
                         { Type = (uint) windowEventListenedMessagesTypes[2], Data = new[] { dataParamTest } });
            Assert.IsTrue(consumedEvent.WaitOne(MaxWaitTime), "Did not find a matching window event in time.");
        }

        [TestMethod]
        public async Task WindowStateChangedEventProducerCallbackTest_Minimized()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowStateChangedEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            const int dataParamTest = 5;
            var callback = GetCallback();

            var consumedEvent = new ManualResetEvent(false);

            var task = new Task(() => findMatch(windowStateChangedEventProducer, consumedEvent, @event =>
                                                    @event.Title.Equals("sampleStateChangedTitle") 
                                                    && @event.ProcessName.Equals("sampleProcessName")
                                                    && @event.WindowState == WindowState.Minimized));
            task.Start();

            callback(new GlobalHook.HookMessage
            {
                Type = (uint) windowEventListenedMessagesTypes[3], Data = new[] { dataParamTest },
                wParam = (IntPtr) WindowState.Minimized
            });
            Assert.IsTrue(consumedEvent.WaitOne(MaxWaitTime), "Did not find a matching window event in time.");
        }

        private async void findMatch<T>(DefaultEventQueue<T> producer, ManualResetEvent reset, Func<T, bool> predicate)
            where T : WindowEvent
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

        private GlobalHook.CppGetMessageCallback GetCallback()
        {
            GlobalHook.CppGetMessageCallback callback = null;
            foreach (var messageType in windowEventListenedMessagesTypes)
            {
                hookNativeMethods.AllowMessageTypeRegistry(messageType);
            }

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
            windowManagementModule.Initialize(true);
            windowManagementModule.IsActive = true;

            //wait for the hookNativeMethodsMock.Mock.Callback is called!
            Assert.IsTrue(callbackReceivedEvent.WaitOne(MaxWaitTime), "Did not receive callback in time!");
            Assert.IsNotNull(callback, "Callback received however unexpectedly null!");
            return callback;
        }
    }
}