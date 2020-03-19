using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.WindowManagement;
using MORR.Modules.WindowManagement.Events;
using MORR.Modules.WindowManagement.Producers;
using MORR.Shared.Hook;
using SharedTest.TestHelpers.INativeHook;

namespace WindowManagementTest
{
    [TestClass]
    public class WindowStateChangedEventProducerTest
    {
        protected const int MaxWaitTime = 500;

        private CompositionContainer container;
        private HookNativeMethodsMock hookNativeMethods;
        private NativeWindowMock nativeWindowManagement;
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

            nativeWindowManagement = new NativeWindowMock();

            hookNativeMethods = new HookNativeMethodsMock();
            hookNativeMethods.Initialize();
        }

        [TestCleanup]
        public void AfterTest()
        {
            windowManagementModule = null;
            windowFocusEventProducer = null;
            nativeWindowManagement = null;
            windowMovementEventProducer = null;
            windowResizingEventProducer = null;
            windowStateChangedEventProducer = null;
            container.Dispose();
            container = null;
            hookNativeMethods = null;
            nativeWindowManagement = null;
        }

        [TestMethod]
        public void WindowStateChangedEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowStateChangedEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);
            Debug.Assert(nativeWindowManagement != null);

            /* GIVEN */
            var callback = GetCallback();

            nativeWindowManagement.GetProcessName();
            nativeWindowManagement.GetTitle();

            GlobalHook.HookMessage[] hookMessages =
            {
                new GlobalHook.HookMessage
                {
                    Type = (uint) GlobalHook.MessageType.WM_SIZE,
                    wParam = (IntPtr) WindowState.Maximized
                },
                new GlobalHook.HookMessage
                {
                    Type = (uint) GlobalHook.MessageType.WM_SIZE,
                    wParam = (IntPtr) WindowState.Minimized
                },
                new GlobalHook.HookMessage
                    { Type = (uint) GlobalHook.MessageType.WM_ENTERSIZEMOVE, Hwnd = (IntPtr) 1 },
                new GlobalHook.HookMessage
                    { Type = (uint) GlobalHook.MessageType.WM_EXITSIZEMOVE, Hwnd = (IntPtr) 1 }
            };

            WindowStateChangedEvent[] expectedEvents =
            {
                new WindowStateChangedEvent
                {
                    ProcessName = "ProcessName",
                    Title = "Title",
                    WindowState = WindowState.Maximized
                },

                new WindowStateChangedEvent
                {
                    ProcessName = "ProcessName",
                    Title = "Title",
                    WindowState = WindowState.Minimized
                },
                new WindowStateChangedEvent
                {
                    ProcessName = "ProcessName",
                    Title = "Title",
                    WindowState = WindowState.Normal
                }
            };
            Assert.IsTrue(hookMessages.Length == expectedEvents.Length + 1);

            /* WHEN */
            using var consumedEvent = new CountdownEvent(expectedEvents.Length);
            Assert.IsTrue(consumedEvent.CurrentCount == expectedEvents.Length);
            using var didStartConsumingEvent = new ManualResetEvent(false);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await(
                    windowStateChangedEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!isWindowStateChangedEventFound(@event, expectedEvents))
                    {
                        continue;
                    }

                    consumedEvent.Signal();
                }
            });

            thread.Start();

            Assert.IsTrue(didStartConsumingEvent.WaitOne(MaxWaitTime));

            foreach (var msg in hookMessages)
            {
                callback(msg);
            }

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime),
                          "Did not find all matching window state changed event in time.");

            windowStateChangedEventProducer.StopCapture();
            windowManagementModule.Initialize(false);
        }

        private GlobalHook.CppGetMessageCallback GetCallback()
        {
            GlobalHook.CppGetMessageCallback callback = null;
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_SIZE);
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_ENTERSIZEMOVE);
            hookNativeMethods.AllowMessageTypeRegistry(GlobalHook.MessageType.WM_EXITSIZEMOVE);

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
            windowStateChangedEventProducer.StartCapture(nativeWindowManagement.Mock.Object);

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

        private bool isWindowStateChangedEventFound(WindowStateChangedEvent @event,
                                                    WindowStateChangedEvent[] expectedEvents)
        {
            foreach (var e in expectedEvents)
            {
                if (@event.Title.Equals(e.Title) && @event.ProcessName.Equals(e.ProcessName) &&
                    @event.WindowState.Equals(e.WindowState))
                {
                    return true;
                }
            }

            return false;
        }
    }
}