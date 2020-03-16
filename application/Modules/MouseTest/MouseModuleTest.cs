using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.Mouse;
using MORR.Modules.Mouse.Events;
using MORR.Modules.Mouse.Producers;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;
using SharedTest.TestHelpers.INativeHook;
using System.Windows;
using MORR.Modules.Mouse.Native;

namespace MouseTest
{
    [TestClass]
    public class MouseModuleTest
    {
        protected const int maxWaitTime = 5000;

        private class TestMouseModuleConfiguration : MouseModuleConfiguration
        {
            public TestMouseModuleConfiguration()
            {
                SamplingRateInHz = 10;
                Threshold = 50;
            }
        }
        private CompositionContainer container;
        private MouseClickEventProducer mouseClickEventProducer;
        private MouseMoveEventProducer mouseMoveEventProducer;
        private MouseScrollEventProducer mouseScrollEventProducer;
        private MouseModule mouseModule;
        private MouseModuleConfiguration mouseModuleConfiguration;
        private Mock<INativeMouse> nativeMouseMock;
        private HookNativeMethodsMock hookNativeMethodsMock;

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

            //initialize the native mouse mock
            nativeMouseMock = new Mock<INativeMouse>();

            //initialzie the hookNativeMethodsMock
            hookNativeMethodsMock = new HookNativeMethodsMock();
            hookNativeMethodsMock.Initialize();
        }

        [TestMethod]
        public void TestMouseModule_ActivateTrue()
        {
            // Preconditions
            Debug.Assert(mouseModule != null);

            /* GIVEN */

            /* WHEN */
            mouseModule.Initialize(true);
            AllowMessageTypeRegistryForAll();
            hookNativeMethodsMock.AllowLibraryLoad();
            mouseModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(mouseModule.IsActive);
        }

        [TestMethod]
        public void TestMouseModule_ActivateFalse()
        {
            // Preconditions
            Debug.Assert(mouseModule != null);

            /* GIVEN */

            /* WHEN */
            mouseModule.Initialize(true);
            AllowMessageTypeRegistryForAll();
            hookNativeMethodsMock.AllowLibraryLoad();
            mouseModule.IsActive = true;
            mouseModule.IsActive = false;

            /* THEN */
            Assert.IsFalse(mouseModule.IsActive);
        }

        [TestMethod]
        public void TestMouseModule_InitializeFalse()
        {
            // Preconditions
            Debug.Assert(mouseModule != null);

            /* GIVEN */

            /* WHEN */
            mouseModule.Initialize(false);
            /* THEN */
            Assert.IsTrue(mouseClickEventProducer.IsClosed);
            Assert.IsTrue(mouseMoveEventProducer.IsClosed);
            Assert.IsTrue(mouseScrollEventProducer.IsClosed);
        }

        [TestMethod]
        public void TestMouseModuleInitializeTrue()
        {
            // Preconditions
            Debug.Assert(mouseModule != null);

            /* GIVEN */

            /* WHEN */
            mouseModule.Initialize(true);

            /* THEN */
            Assert.IsFalse(mouseClickEventProducer.IsClosed);
            Assert.IsFalse(mouseMoveEventProducer.IsClosed);
            Assert.IsFalse(mouseScrollEventProducer.IsClosed);
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
            GlobalHook.CppGetMessageCallback callback = GetCallback();
            //setting up fake messages and corresponding expected Events
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

            /* WHEN */
            //Running the task in another thread
            var consumedEvent = new CountdownEvent(hookMessages.Length);
            var task = new Task(() => findMatch(mouseClickEventProducer, consumedEvent, expectedEvents, isMouseClickEventFound));
            task.Start();
            // We must call the callback after we start the consumer for the producer.
            // otherwise the message is automatically dismissed.
            foreach (GlobalHook.HookMessage message in hookMessages)
            {
                callback(message);
            }

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(maxWaitTime), "Did not find all matching mouse click events in time.");
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
            GlobalHook.CppGetMessageCallback callback = GetCallback();
            //setting up fake messages and corresponding expected Events
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

            /* WHEN */
            //Running the task in another thread
            var consumedEvent = new CountdownEvent(hookMessages.Length);
            var task = new Task(() => findMatch(mouseScrollEventProducer, consumedEvent, expectedEvents, isMouseScrollEventFound));
            task.Start();
            // We must call the callback after we start the consumer for the producer.
            // otherwise the message is automatically dismissed.
            foreach (GlobalHook.HookMessage message in hookMessages)
            {
                callback(message);
            }

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(maxWaitTime), "Did not find all matching mouse scroll events in time.");
        }

        [TestMethod]
        public void TestMouseMoveEventProducer_Callback()
        {
            /* PRECONDITIONS */
            Debug.Assert(mouseModule != null);
            Debug.Assert(mouseMoveEventProducer != null);
            Debug.Assert(nativeMouseMock != null);

            /* GIVEN */
            // setting up GetCursorPos(): first time returns 100,100 second time 0,0 
            nativeMouseMock.SetupSequence(nM => nM.GetCursorPos())
                .Returns(new INativeMouse.POINT { X = 100, Y = 100 })
                .Returns(new INativeMouse.POINT { X = 0, Y = 0 });

            // since the the cursor position varis from 100,100 to 0,0 (> threshold), we expect to
            // receive event with position 0,0
            MouseMoveEvent[] expectedEvents = {
            new MouseMoveEvent {MousePosition = new Point { X = 0, Y = 0}},
            };

            /* WHEN */
            //Running the task in another thread
            var consumedEvent = new CountdownEvent(1);
            var task = new Task(() => findMatch(mouseMoveEventProducer, consumedEvent, expectedEvents, isMouseMoveEventFound));
            task.Start();

            // God of code Niklas: "We must call the callback after we start the consumer for the producer.
            // otherwise the message is automatically dismissed."
            // in this case, first the task.Start() above then the mouseMoveEventProducer.StartCapture(nativeMouseMock.Object) under.

            // open up the strategy channel
            mouseModule.Initialize(true);

            /// this is where the GetMousePosition() "callback" in the producer will be called! with Timer
            ///!!! there seems to be race conditions with the GetMousePosition() of producer class in the test.!!!///
            /// I set a break point in the GetMousePosition() Method and found that multiple threads are executing this method!///
            mouseMoveEventProducer.StartCapture(nativeMouseMock.Object);

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(maxWaitTime), "Did not find all matching mouse move events in time.");
        }

        /// <summary>
        ///     See if all the events generated by the producer machtes the expectation
        /// </summary>
        /// <typeparam name="T">The type of event which will be captured.</typeparam>
        /// <param name="producer">The producer which generates the events.</param>
        /// <param name="countdown">A reset event to identify if all the predicate was met.param>
        /// <param name="expectedEvents">an array of events that is expected to be generated by the producer</param>
        /// <param name="predicate">A predicate which should return true if the expected event has been received.</param>
        private async void findMatch<T>(DefaultEventQueue<T> producer, CountdownEvent countdown, T[] expectedEvents, Func<T, T[], bool> predicate) where T : MouseEvent
        {
            await foreach (var @event in producer.GetEvents())
            {
                if (predicate.Invoke(@event, expectedEvents))
                {
                    countdown.Signal();
                }
            }
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
            var callbackReceivedEvent = new AutoResetEvent(false);

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
        private bool isMouseClickEventFound(MouseClickEvent @event, MouseClickEvent[] expectedEvents)
        {
            foreach (MouseClickEvent e in expectedEvents)
            {
                if (@event.MouseAction.Equals(e.MouseAction) && @event.MousePosition.Equals(e.MousePosition) && @event.HWnd.Equals(e.HWnd)) return true;
            }
            return false;
        }

        /// <summary>
        ///     Return true if an mouse scroll event is in the array of expected events.
        ///     This method itslef defines the logic to determine if two mouse scroll events are equal.
        /// </summary>
        /// <param name="event">an event that is expected to be in the array of expected events</param>
        /// <param name="expectedEvents">an array of expected events</param>
        /// <returns>true if an event is indeed in the array of expected events.</returns>
        private bool isMouseScrollEventFound(MouseScrollEvent @event, MouseScrollEvent[] expectedEvents)
        {
            foreach (MouseScrollEvent e in expectedEvents)
            {
                if (@event.ScrollAmount.Equals(e.ScrollAmount) && @event.MousePosition.Equals(e.MousePosition) && @event.HWnd.Equals(e.HWnd)) return true;
            }
            return false;
        }

        /// <summary>
        ///     Return true if an mouse move event is in the array of expected events.
        ///     This method itslef defines the logic to determine if two mouse move events are equal.
        /// </summary>
        /// <param name="event">an event that is expected to be in the array of expected events</param>
        /// <param name="expectedEvents">an array of expected events</param>
        /// <returns>true if an event is indeed in the array of expected events.</returns>
        private bool isMouseMoveEventFound(MouseMoveEvent @event, MouseMoveEvent[] expectedEvents)
        {
            foreach (MouseMoveEvent e in expectedEvents)
            {
                if (@event.MousePosition.Equals(e.MousePosition)) return true;
            }
            return false;
        }
    }
}