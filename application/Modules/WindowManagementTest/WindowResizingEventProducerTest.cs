using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.WindowManagement;
using MORR.Modules.WindowManagement.Producers;
using MORR.Shared.Hook;
using SharedTest.TestHelpers.INativeHook;

namespace WindowManagementTest
{
    [TestClass]
    public class WindowResizingEventProducerTest
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
        public void WindowResizingEventProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(windowManagementModule != null);
            Debug.Assert(windowResizingEventProducer != null);
            Debug.Assert(hookNativeMethods != null);
            Debug.Assert(hookNativeMethods.Mock != null);

            /* GIVEN */
            var callback = GetCallback();

            nativeWindowManagement.GetProcessName();
            nativeWindowManagement.GetTitle();
            nativeWindowManagement.IsRectSizeNotEqual();
            nativeWindowManagement.GetWindowRect();
            nativeWindowManagement.GetWidthAndHeight();

            using var consumedEvent = new CountdownEvent(1);

            using var didStartConsumingEvent = new ManualResetEvent(false);

            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await(
                    windowResizingEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!(@event.Title.Equals("Title")
                          && @event.ProcessName.Equals("ProcessName")
                          && @event.OldSize.Equals(new Size(1, 1))
                          && @event.NewSize.Equals(new Size(2, 2))))
                    {
                        continue;
                    }

                    consumedEvent.Signal();
                }
            });

            thread.Start();

            Assert.IsTrue(didStartConsumingEvent.WaitOne(MaxWaitTime));
            callback(new GlobalHook.HookMessage
                         { Type = (uint) GlobalHook.MessageType.WM_ENTERSIZEMOVE, Hwnd = (IntPtr) 1 });
            callback(new GlobalHook.HookMessage
                         { Type = (uint) GlobalHook.MessageType.WM_EXITSIZEMOVE, Hwnd = (IntPtr) 1 });

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(MaxWaitTime), "Did not find all matching window resizing event in time.");

            windowManagementModule.IsActive = false;
            windowManagementModule.Initialize(false);
        }

        private GlobalHook.CppGetMessageCallback GetCallback()
        {
            GlobalHook.CppGetMessageCallback callback = null;
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
            windowResizingEventProducer.StartCapture(nativeWindowManagement.Mock.Object);
            windowMovementEventProducer.Close();
            windowStateChangedEventProducer.Close();
            ;

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