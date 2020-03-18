using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.WindowManagement;
using MORR.Modules.WindowManagement.Producers;
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
        public void WindowFocusEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowFocusEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);
            Debug.Assert(windowEventListenedMessagesTypes != null);

            /* GIVEN */
            const int WA_ACTIVE = 1;
            const int dataParamTest = 1;

            var callback = GetCallback();

            using var consumedEvent = new CountdownEvent(1);

            using var didStartConsumingEvent = new ManualResetEvent(false);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await(
                    windowFocusEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!(@event.Title.Equals("sampleFocusTitle")
                          && @event.ProcessName.Equals("sampleProcessName")))
                    {
                        continue;
                    }

                    consumedEvent.Signal();
                }
            });

            thread.Start();

            Assert.IsTrue(didStartConsumingEvent.WaitOne(MaxWaitTime));
            callback(new GlobalHook.HookMessage
            {
                Type = (uint) windowEventListenedMessagesTypes[0], wParam = (IntPtr) WA_ACTIVE,
                Data = new[] { dataParamTest }
            });

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime), "Did not find all matching window focus event in time.");

            windowManagementModule.IsActive = false;
            windowManagementModule.Initialize(false);
        }

        [TestMethod]
        public void WindowMovementEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowMovementEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);
            Debug.Assert(windowEventListenedMessagesTypes != null);

            /* GIVEN */
            const int dataParamTest = 2;
            var callback = GetCallback();

            using var consumedEvent = new CountdownEvent(1);

            using var didStartConsumingEvent = new ManualResetEvent(false);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await(
                    windowMovementEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!(@event.Title.Equals("sampleMovementTitle")
                          && @event.ProcessName.Equals("sampleProcessName")
                          && @event.OldLocation.Equals(new Point(0, 0))
                          && @event.NewLocation.Equals(new Point(1, 1))))
                    {
                        continue;
                    }

                    consumedEvent.Signal();
                }
            });

            thread.Start();

            Assert.IsTrue(didStartConsumingEvent.WaitOne(MaxWaitTime));
            callback(new GlobalHook.HookMessage
                         { Type = (uint) windowEventListenedMessagesTypes[1], Hwnd = (IntPtr) 1 });
            callback(new GlobalHook.HookMessage
                         { Type = (uint) windowEventListenedMessagesTypes[2], Data = new[] { dataParamTest } });

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime), "Did not find all matching window movement event in time.");

            windowManagementModule.IsActive = false;
            windowManagementModule.Initialize(false);
        }

        [TestMethod]
        public void WindowResizingEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowResizingEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);
            Debug.Assert(windowEventListenedMessagesTypes != null);

            /* GIVEN */
            const int dataParamTest = 3;
            var callback = GetCallback();

            using var consumedEvent = new CountdownEvent(1);

            using var didStartConsumingEvent = new ManualResetEvent(false);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await(
                    windowResizingEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!(@event.Title.Equals("sampleResizingTitle")
                          && @event.ProcessName.Equals("sampleProcessName")
                          && @event.OldSize.Equals(new Size(0, 0))
                          && @event.NewSize.Equals(new Size(1, 1))))
                    {
                        continue;
                    }

                    consumedEvent.Signal();
                }
            });

            thread.Start();

            Assert.IsTrue(didStartConsumingEvent.WaitOne(MaxWaitTime));
            callback(new GlobalHook.HookMessage
                         { Type = (uint) windowEventListenedMessagesTypes[1], Hwnd = (IntPtr) 1 });
            callback(new GlobalHook.HookMessage
                         { Type = (uint) windowEventListenedMessagesTypes[2], Data = new[] { dataParamTest } });

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime), "Did not find all matching window resizing event in time.");

            windowManagementModule.IsActive = false;
            windowManagementModule.Initialize(false);
        }

        [TestMethod]
        public void WindowStateChangedEventProducerCallbackTest_Restored()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowStateChangedEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);
            Debug.Assert(windowEventListenedMessagesTypes != null);

            /* GIVEN */
            const int dataParamTest = 4;
            var callback = GetCallback();

            using var consumedEvent = new CountdownEvent(1);

            using var didStartConsumingEvent = new ManualResetEvent(false);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await(
                    windowStateChangedEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!(@event.Title.Equals("sampleStateChangedTitle")
                          && @event.ProcessName.Equals("sampleProcessName")
                          && @event.WindowState == WindowState.Normal))
                    {
                        continue;
                    }

                    consumedEvent.Signal();
                }
            });

            thread.Start();

            Assert.IsTrue(didStartConsumingEvent.WaitOne(MaxWaitTime));
            callback(new GlobalHook.HookMessage
                         { Type = (uint) windowEventListenedMessagesTypes[1], Hwnd = (IntPtr) 1 });
            callback(new GlobalHook.HookMessage
                         { Type = (uint) windowEventListenedMessagesTypes[2], Data = new[] { dataParamTest } });

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime),
                          "Did not find all matching window state changed event in time.");

            windowManagementModule.IsActive = false;
            windowManagementModule.Initialize(false);
        }

        [TestMethod]
        public void WindowStateChangedEventProducerCallbackTest_Minimized()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowStateChangedEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);
            Debug.Assert(windowEventListenedMessagesTypes != null);

            /* GIVEN */
            const int dataParamTest = 5;
            var callback = GetCallback();

            using var consumedEvent = new CountdownEvent(1);

            using var didStartConsumingEvent = new ManualResetEvent(false);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await(
                    windowStateChangedEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!(@event.Title.Equals("sampleStateChangedTitle")
                          && @event.ProcessName.Equals("sampleProcessName")
                          && @event.WindowState == WindowState.Minimized))
                    {
                        continue;
                    }

                    consumedEvent.Signal();
                }
            });

            thread.Start();

            Assert.IsTrue(didStartConsumingEvent.WaitOne(MaxWaitTime));
            callback(new GlobalHook.HookMessage
            {
                Type = (uint) windowEventListenedMessagesTypes[3], Data = new[] { dataParamTest },
                wParam = (IntPtr) WindowState.Minimized
            });

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime),
                          "Did not find all matching window state changed event in time.");

            windowManagementModule.IsActive = false;
            windowManagementModule.Initialize(false);
        }

        [TestMethod]
        public void WindowStateChangedEventProducerCallbackTest_Maximized()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowStateChangedEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);
            Debug.Assert(windowEventListenedMessagesTypes != null);

            /* GIVEN */
            const int dataParamTest = 6;
            var callback = GetCallback();

            using var consumedEvent = new CountdownEvent(1);

            using var didStartConsumingEvent = new ManualResetEvent(false);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await(
                    windowStateChangedEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!(@event.Title.Equals("sampleStateChangedTitle")
                          && @event.ProcessName.Equals("sampleProcessName")
                          && @event.WindowState == WindowState.Maximized))
                    {
                        continue;
                    }

                    consumedEvent.Signal();
                }
            });

            thread.Start();

            Assert.IsTrue(didStartConsumingEvent.WaitOne(MaxWaitTime));
            callback(new GlobalHook.HookMessage
            {
                Type = (uint) windowEventListenedMessagesTypes[3], Data = new[] { dataParamTest },
                wParam = (IntPtr) WindowState.Maximized
            });

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime),
                          "Did not find all matching window state changed event in time.");

            windowManagementModule.IsActive = false;
            windowManagementModule.Initialize(false);
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