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
using System.Threading;
using Moq;
using System.Windows;
using MORR.Modules.Mouse.Native;

namespace MouseTest.Producers
{
    [TestClass]
    public class MouseMoveEventProducerTest
    {
        protected const int maxWaitTime = 5000;

        private CompositionContainer container;
        private MouseClickEventProducer mouseClickEventProducer;
        private MouseMoveEventProducer mouseMoveEventProducer;
        private MouseScrollEventProducer mouseScrollEventProducer;
        private MouseModule mouseModule;
        private MouseModuleConfiguration mouseModuleConfiguration;
        private HookNativeMethodsMock hookNativeMethodsMock;
        private Mock<INativeMouse> nativeMouseMock;
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

            //initialize the native mouse mock
            nativeMouseMock = new Mock<INativeMouse>();

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
            nativeMouseMock = null;
        }

        [TestMethod]
        public void TestMouseMoveEventProducer_Callback()
        {
            /* PRECONDITIONS */
            Debug.Assert(mouseModule != null);
            Debug.Assert(mouseMoveEventProducer != null);
            Debug.Assert(hookNativeMethodsMock != null);
            Debug.Assert(hookNativeMethodsMock.Mock != null);

            /* GIVEN */
            mouseModule.Initialize(true);

            //setting up fake nativeMouseMock behaviors, messages and corresponding expected Events
            nativeMouseMock.SetupSequence(nM => nM.GetCursorPos())
                .Returns(new INativeMouse.POINT { X = 100, Y = 100 })
                .Returns(new INativeMouse.POINT { X = 0, Y = 0 });

            // since the the cursor position varis from 100,100 to 0,0 (> threshold), we expect to
            // receive event with position 0,0
            MouseMoveEvent[] expectedEvents = {
            new MouseMoveEvent {MousePosition = new Point { X = 0, Y = 0}}
            };

            /* WHEN */
            // consumedEvent.Signal() will be called gdw one event has been found and meet expectation
            using var consumedEvent = new CountdownEvent(expectedEvents.Length);
            Assert.IsTrue(consumedEvent.CurrentCount == expectedEvents.Length);

            //didStartConsumingEvent.Set() will be called in Await method gdw the consumer is attached
            using var didStartConsumingEvent = new ManualResetEvent(false);

            //Running the task in another thread
            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await<IAsyncEnumerable<MouseMoveEvent>>(mouseMoveEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!IsMouseMoveEventFound(@event, expectedEvents))
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
            /// this is where the GetMousePosition() "callback" in the producer will be called! with Timer
            mouseMoveEventProducer.StartCapture(nativeMouseMock.Object);

            /* THEN */
            //true if all events generated by the fake messages have meet expectation.
            Assert.IsTrue(consumedEvent.Wait(maxWaitTime), "Did not find all matching mouse move events in time.");

            //total shut down and resources release
            mouseMoveEventProducer.StopCapture();
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
        ///     Return true if an mouse move event is in the array of expected events.
        ///     This method itslef defines the logic to determine if two mouse move events are equal.
        /// </summary>
        /// <param name="event">an event that is expected to be in the array of expected events</param>
        /// <param name="expectedEvents">an array of expected events</param>
        /// <returns>true if an event is indeed in the array of expected events.</returns>
        private bool IsMouseMoveEventFound(MouseMoveEvent @event, MouseMoveEvent[] expectedEvents)
        {
            foreach (MouseMoveEvent e in expectedEvents)
            {
                if (@event.MousePosition.Equals(e.MousePosition)) return true;
            }
            return false;
        }
    }
}
