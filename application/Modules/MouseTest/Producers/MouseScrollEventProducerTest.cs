using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Modules.Mouse;
using MORR.Modules.Mouse.Events;
using MORR.Modules.Mouse.Producers;
using MORR.Shared.Hook;
using SharedTest.TestHelpers.INativeHook;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading;
using Moq;
using System.Windows;

namespace MouseTest.Producers
{
    [TestClass]
    public class MouseScrollEventProducerTest
    {
        protected const int maxWaitTime = 5000;

        private CompositionContainer container;
        private MouseClickEventProducer mouseClickEventProducer;
        private MouseMoveEventProducer mouseMoveEventProducer;
        private MouseScrollEventProducer mouseScrollEventProducer;
        private MouseModule mouseModule;
        private MouseModuleConfiguration mouseModuleConfiguration;
        private HookNativeMethodsMock hookNativeMethodsMock;
        private class TestMouseModuleConfiguration : MouseModuleConfiguration
        {
            public TestMouseModuleConfiguration()
            {
                SamplingRateInHz = 10;
                Threshold = 50;
            }
        }

        private readonly GlobalHook.MessageType[] mouseClickListenedMessagesTypes =
        {
            GlobalHook.MessageType.WM_RBUTTONDOWN,
            GlobalHook.MessageType.WM_LBUTTONDOWN,
            GlobalHook.MessageType.WM_MBUTTONDOWN,
            GlobalHook.MessageType.WM_RBUTTONDBLCLK,
            GlobalHook.MessageType.WM_LBUTTONDBLCLK,
            GlobalHook.MessageType.WM_MBUTTONDBLCLK,
            GlobalHook.MessageType.WM_NCRBUTTONDOWN,
            GlobalHook.MessageType.WM_NCLBUTTONDOWN,
            GlobalHook.MessageType.WM_NCMBUTTONDOWN,
            GlobalHook.MessageType.WM_NCRBUTTONDBLCLK,
            GlobalHook.MessageType.WM_NCLBUTTONDBLCLK,
            GlobalHook.MessageType.WM_NCMBUTTONDBLCLK
        };

        private readonly GlobalHook.MessageType[] mouseScrollListenedMessagesTypes =
        {
            GlobalHook.MessageType.WM_MOUSEWHEEL
        };

        [TestInitialize]
        public void BeforeTest()
        {
            //initialize module, producers and configuration
            mouseModule = new MouseModule();
            mouseClickEventProducer = new MouseClickEventProducer();
            mouseMoveEventProducer = new MouseMoveEventProducer();
            mouseScrollEventProducer = new MouseScrollEventProducer();
            mouseModuleConfiguration = new TestMouseModuleConfiguration();

            // initialize the container and fulfill the MEF inports exports
            container = new CompositionContainer();
            container.ComposeExportedValue(mouseClickEventProducer);
            container.ComposeExportedValue(mouseMoveEventProducer);
            container.ComposeExportedValue(mouseScrollEventProducer);
            container.ComposeExportedValue(mouseModuleConfiguration);
            container.ComposeParts(mouseModule);

            //initialzie the hookNativeMethodsMock
            hookNativeMethodsMock = new HookNativeMethodsMock();
            hookNativeMethodsMock.Initialize();
        }

        [TestCleanup]
        public void AfterTest()
        {
            // null everything!
            mouseModule = null;
            mouseClickEventProducer = null;
            mouseMoveEventProducer = null;
            mouseScrollEventProducer = null;
            mouseModuleConfiguration = null;
            container.Dispose();
            container = null;
            hookNativeMethodsMock = null;
        }

        [TestMethod]
        public void TestMouseScrollEventProducer_Callback()
        {
            /* PRECONDITIONS */
            Debug.Assert(mouseModule != null);
            Debug.Assert(mouseScrollEventProducer != null);
            Debug.Assert(hookNativeMethodsMock != null);
            Debug.Assert(hookNativeMethodsMock.Mock != null);

            /* GIVEN */
            //get the callback
            GlobalHook.CppGetMessageCallback callback = GetCallback();

            //setting up fake nativeMouseMock behaviors, messages and corresponding expected Events
            GlobalHook.HookMessage[] hookMessages = {
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_MOUSEWHEEL, Hwnd = (IntPtr)1, wParam = (IntPtr)0x00780000, Data = new int[] { 10, 100 }},
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_MOUSEWHEEL, Hwnd = (IntPtr)2, wParam = (IntPtr)0x00080000, Data = new int[] { 20, 200 }},
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_MOUSEWHEEL, Hwnd = (IntPtr)23, wParam = (IntPtr)0xfff80000, Data = new int[] { 54, 23 }},
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_MOUSEWHEEL, Hwnd = (IntPtr)43, wParam = (IntPtr)0xff880000, Data = new int[] { 33, 101 }}
            };
            MouseScrollEvent[] expectedEvents = {
            new MouseScrollEvent { ScrollAmount = 120, HWnd = "1", MousePosition = new Point { X = 10, Y = 100 }},
            new MouseScrollEvent { ScrollAmount = 8, HWnd = "2", MousePosition = new Point { X = 20, Y = 200 }},
            new MouseScrollEvent { ScrollAmount = -8, HWnd = "23", MousePosition = new Point { X = 54, Y = 23 }},
            new MouseScrollEvent { ScrollAmount = -120, HWnd = "43", MousePosition = new Point { X = 33, Y = 101 }}
            };

            Assert.IsTrue(hookMessages.Length == expectedEvents.Length);

            /* WHEN */
            // consumedEvent.Signal() will be called gdw one event has been found and meet expectation
            using var consumedEvent = new CountdownEvent(hookMessages.Length);
            Assert.IsTrue(consumedEvent.CurrentCount == hookMessages.Length);

            //didStartConsumingEvent.Set() will be called in Await method gdw the consumer is attached
            using var didStartConsumingEvent = new ManualResetEvent(false);

            //Running the task in another thread
            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await<IAsyncEnumerable<MouseScrollEvent>>(mouseScrollEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!IsMouseScrollEventFound(@event, expectedEvents))
                    {
                        continue;
                    }
                    consumedEvent.Signal();
                }
            });
            thread.Start();
            // true if the consumer is attached!
            Assert.IsTrue(didStartConsumingEvent.WaitOne(maxWaitTime));

            // We must call the callback after the consumer is attached!
            // otherwise the message is automatically dismissed.
            foreach (GlobalHook.HookMessage message in hookMessages)
            {
                callback(message);
            }

            /* THEN */
            //true if all events generated by the fake messages have meet expectation.
            Assert.IsTrue(consumedEvent.Wait(maxWaitTime), "Did not find all matching mouse scroll events in time.");

            //total shut down and resources release
            mouseModule.IsActive = false;
            mouseModule.Initialize(false);
        }

        /// <summary>
        ///     Set the ManualResetEvent to indicate that the consumer has been attached.
        ///     Then return the awaitedObject, which in this case should be an 
        ///     IAsyncEnumerable<SomeEvent> Type.
        /// </summary>
        /// <typeparam name="T">The type of the returned object, which in this case should be an IAsyncEnumerable<SomeEvent> </typeparam>
        /// <param name="awaitedObject">the object to "await foreach", which in this case should be of type IAsyncEnumerable<SomeEvent> </param>
        /// <param name="expectation">a ManualRestEvent to Set to indicate that the consumer has been attached</param>
        /// <returns></returns>
        public static T Await<T>(T awaitedObject, ManualResetEvent expectation)
        {
            expectation.Set();
            return awaitedObject;
        }

        /// <summary>
        ///     Call AllowMessageTypeRegistry() methods on all messages related to the mouse producers.
        /// </summary>
        private void AllowMessageTypeRegistryForAll()
        {
            foreach (GlobalHook.MessageType messageType in mouseClickListenedMessagesTypes)
            {
                hookNativeMethodsMock.AllowMessageTypeRegistry(messageType);
            }
            foreach (GlobalHook.MessageType messageType in mouseScrollListenedMessagesTypes)
            {
                hookNativeMethodsMock.AllowMessageTypeRegistry(messageType);
            }
        }

        /// <summary>
        ///     Performs a series of initialization and Setups to get the CppGetMessageCallback.
        /// </summary>
        /// <returns>the callback that can be called with a message, which in turns calls a callback in the producers that is interested in this type of message</returns>
        private GlobalHook.CppGetMessageCallback GetCallback()
        {
            GlobalHook.CppGetMessageCallback callback = null;
            AllowMessageTypeRegistryForAll();
            hookNativeMethodsMock.AllowLibraryLoad();
            using var callbackReceivedEvent = new AutoResetEvent(false);

            hookNativeMethodsMock.Mock
                 .Setup(hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()))?
                 .Callback((GlobalHook.CppGetMessageCallback cppCallback, bool isBlocking) =>
                 {
                     callback = cppCallback;
                     callbackReceivedEvent.Set();
                 });
            //here the SetHook() method is called!
            mouseModule.Initialize(true);
            mouseModule.IsActive = true;

            //wait for the hookNativeMethodsMock.Mock.Callback is called!
            Assert.IsTrue(callbackReceivedEvent.WaitOne(maxWaitTime), "Did not receive callback in time!");
            Assert.IsNotNull(callback, "Callback received however unexpectedly null!");
            return callback;
        }

        /// <summary>
        ///     Return true if an mouse scroll event is in the array of expected events.
        ///     This method itslef defines the logic to determine if two mouse scroll events are equal.
        /// </summary>
        /// <param name="event">an event that is expected to be in the array of expected events</param>
        /// <param name="expectedEvents">an array of expected events</param>
        /// <returns>true if an event is indeed in the array of expected events.</returns>
        private bool IsMouseScrollEventFound(MouseScrollEvent @event, MouseScrollEvent[] expectedEvents)
        {
            foreach (MouseScrollEvent e in expectedEvents)
            {
                if (@event.ScrollAmount.Equals(e.ScrollAmount) && @event.MousePosition.Equals(e.MousePosition) && @event.HWnd.Equals(e.HWnd)) return true;
            }
            return false;
        }
    }
}
