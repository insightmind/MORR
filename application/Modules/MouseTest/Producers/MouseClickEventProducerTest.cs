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
    public class MouseClickEventProducerTest
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
        public void TestMouseClickEventProducer_Callback()
        {
            /* PRECONDITIONS */
            Debug.Assert(mouseModule != null);
            Debug.Assert(mouseClickEventProducer != null);
            Debug.Assert(hookNativeMethodsMock != null);
            Debug.Assert(hookNativeMethodsMock.Mock != null);

            /* GIVEN */
            //get the callback
            GlobalHook.CppGetMessageCallback callback = GetCallback();

            //setting up fake nativeMouseMock behaviors, messages and corresponding expected Events
            GlobalHook.HookMessage[] hookMessages = {
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_RBUTTONDOWN, Hwnd = (IntPtr)1, Data = new int[] { 1, 1 } },
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_LBUTTONDOWN, Hwnd = (IntPtr)2, Data = new int[] { 2, 2 } },
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_MBUTTONDOWN, Hwnd = (IntPtr)3, Data = new int[] { 3, 3 } },
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_RBUTTONDBLCLK, Hwnd = (IntPtr)4, Data = new int[] { 4, 4 } },
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_LBUTTONDBLCLK, Hwnd = (IntPtr)5, Data = new int[] { 5, 5 } },
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_MBUTTONDBLCLK, Hwnd = (IntPtr)6, Data = new int[] { 6, 6 } },
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_NCRBUTTONDOWN, Hwnd = (IntPtr)7, Data = new int[] { 7, 7 } },
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_NCLBUTTONDOWN, Hwnd = (IntPtr)8, Data = new int[] { 8, 8 } },
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_NCMBUTTONDOWN, Hwnd = (IntPtr)9, Data = new int[] { 9, 9 } },
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_NCRBUTTONDBLCLK, Hwnd = (IntPtr)10, Data = new int[] { 10, 10 } },
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_NCLBUTTONDBLCLK, Hwnd = (IntPtr)11, Data = new int[] { 11, 11 } },
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_NCMBUTTONDBLCLK, Hwnd = (IntPtr)12, Data = new int[] { 12, 12 } }
            };
            MouseClickEvent[] expectedEvents = {
            new MouseClickEvent { MouseAction = MouseAction.RightClick, HWnd = "1", MousePosition = new Point { X = 1, Y = 1 } },
            new MouseClickEvent { MouseAction = MouseAction.LeftClick, HWnd = "2", MousePosition = new Point { X = 2, Y = 2 } },
            new MouseClickEvent { MouseAction = MouseAction.MiddleClick, HWnd = "3", MousePosition = new Point { X = 3, Y = 3 } },
            new MouseClickEvent { MouseAction = MouseAction.RightDoubleClick, HWnd = "4", MousePosition = new Point { X = 4, Y = 4 } },
            new MouseClickEvent { MouseAction = MouseAction.LeftDoubleClick, HWnd = "5", MousePosition = new Point { X = 5, Y = 5 } },
            new MouseClickEvent { MouseAction = MouseAction.MiddleDoubleClick, HWnd = "6", MousePosition = new Point { X = 6, Y = 6 } },
            new MouseClickEvent { MouseAction = MouseAction.RightClick, HWnd = "7", MousePosition = new Point { X = 7, Y = 7 } },
            new MouseClickEvent { MouseAction = MouseAction.LeftClick, HWnd = "8", MousePosition = new Point { X = 8, Y = 8 } },
            new MouseClickEvent { MouseAction = MouseAction.MiddleClick, HWnd = "9", MousePosition = new Point { X = 9, Y = 9 } },
            new MouseClickEvent { MouseAction = MouseAction.RightDoubleClick, HWnd = "10", MousePosition = new Point { X = 10, Y = 10 } },
            new MouseClickEvent { MouseAction = MouseAction.LeftDoubleClick, HWnd = "11", MousePosition = new Point { X = 11, Y = 11 } },
            new MouseClickEvent { MouseAction = MouseAction.MiddleDoubleClick, HWnd = "12", MousePosition = new Point { X = 12, Y = 12 } }
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
                await foreach (var @event in Await<IAsyncEnumerable<MouseClickEvent>>(mouseClickEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!IsMouseClickEventFound(@event, expectedEvents))
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
            Assert.IsTrue(consumedEvent.Wait(maxWaitTime), "Did not find all matching mouse click events in time.");

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
        ///     Return true if an mouse click event is in the array of expected events.
        ///     This method itslef defines the logic to determine if two mouse click events are equal.
        /// </summary>
        /// <param name="event">an event that is expected to be in the array of expected events</param>
        /// <param name="expectedEvents">an array of expected events</param>
        /// <returns>true if an event is indeed in the array of expected events.</returns>
        private bool IsMouseClickEventFound(MouseClickEvent @event, MouseClickEvent[] expectedEvents)
        {
            foreach (MouseClickEvent e in expectedEvents)
            {
                if (@event.MouseAction.Equals(e.MouseAction) && @event.MousePosition.Equals(e.MousePosition) && @event.HWnd.Equals(e.HWnd)) return true;
            }
            return false;
        }
    }
}
