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

        private readonly GlobalHook.MessageType[] mouseMoveListenedMessagesTypes =
        {
            GlobalHook.MessageType.WM_MOUSEWHEEL
        };


        [TestInitialize]
        public void BeforeTest()
        {
            mouseModule = new MouseModule();
            mouseClickEventProducer = new MouseClickEventProducer();
            mouseMoveEventProducer = new MouseMoveEventProducer();
            mouseScrollEventProducer = new MouseScrollEventProducer();
            mouseModuleConfiguration = new TestMouseModuleConfiguration();

            container = new CompositionContainer();
            container.ComposeExportedValue(mouseClickEventProducer);
            container.ComposeExportedValue(mouseMoveEventProducer);
            container.ComposeExportedValue(mouseScrollEventProducer);
            container.ComposeExportedValue(mouseModuleConfiguration);
            container.ComposeParts(mouseModule);

            hookNativeMethodsMock = new HookNativeMethodsMock();
            hookNativeMethodsMock.Initialize();
        }

        [TestMethod]
        public void ModuleActivateTrue_ModuleActivated()
        {
            // Preconditions
            Debug.Assert(mouseModule != null);

            /* GIVEN */

            /* WHEN */
            mouseModule.Initialize(true);
            foreach (GlobalHook.MessageType messageType in mouseClickListenedMessagesTypes)
            {
                hookNativeMethodsMock.AllowMessageTypeRegistry(messageType);
            }
            foreach (GlobalHook.MessageType messageType in mouseScrollListenedMessagesTypes)
            {
                hookNativeMethodsMock.AllowMessageTypeRegistry(messageType);
            }
            hookNativeMethodsMock.AllowLibraryLoad();
            mouseModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(mouseModule.IsActive);
        }

        [TestMethod]
        public void ModuleActivateFalse_ModuleDeactivated()
        {
            // Preconditions
            Debug.Assert(mouseModule != null);

            /* GIVEN */

            /* WHEN */
            mouseModule.Initialize(true);
            mouseModule.IsActive = false;

            /* THEN */
            Assert.IsFalse(mouseModule.IsActive);
        }

        [TestMethod]
        public async Task ModuleInitializeFalse_ChannelClosed()
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
        public async Task ModuleInitializeTrue_ChannelOpened()
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
        public void MouseClickProducerCallbackTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(mouseModule != null);
            Debug.Assert(mouseClickEventProducer != null);
            Debug.Assert(hookNativeMethodsMock != null);
            Debug.Assert(hookNativeMethodsMock.Mock != null);

            /* GIVEN */
            GlobalHook.CppGetMessageCallback callback = null;

            foreach (GlobalHook.MessageType messageType in mouseClickListenedMessagesTypes)
            {
                hookNativeMethodsMock.AllowMessageTypeRegistry(messageType);
            }

            foreach (GlobalHook.MessageType messageType in mouseScrollListenedMessagesTypes)
            {
                hookNativeMethodsMock.AllowMessageTypeRegistry(messageType);
            }

            hookNativeMethodsMock.AllowLibraryLoad();

            // !CHANGED! Renamed to improve readability
            var callbackReceivedEvent = new AutoResetEvent(false);

            hookNativeMethodsMock.Mock
                 .Setup(hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()))?
                 .Callback((GlobalHook.CppGetMessageCallback cppCallback, bool isBlocking) =>
                 {
                     callback = cppCallback;
                     callbackReceivedEvent.Set();
                 });

            /* WHEN */
            //here the SetHook() method is called!
            mouseModule.Initialize(true);
            mouseModule.IsActive = true;

            //wait for the hookNativeMethodsMock.Mock.Callback is called!
            Assert.IsTrue(callbackReceivedEvent.WaitOne(maxWaitTime), "Did not receive callback in time!");
            Assert.IsNotNull(callback, "Callback received however unexpectedly null!");

            //we can manipulate call back here

            //setting up a fake message
            int[] data = { 1, 1 };
            GlobalHook.HookMessage hookMessage = new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_RBUTTONDOWN, Hwnd = (IntPtr)1, Data = data };

            // !CHANGED! Renamed to consumeEvent to improve readability
            //Running the task in another thread
            var consumedEvent = new ManualResetEvent(false);

            // !CHANGED! We need to run the task in an async way. Therefore we do not use the await keyword
            // otherwise this blocks.
            var task = new Task(() => findMatch(mouseClickEventProducer, consumedEvent, (@event) => @event.MouseAction.Equals(MouseAction.RightClick)));
            task.Start();

            // !CHANGED! We must call the callback after we start the consumer for the producer.
            // otherwise the message is automatically dismissed.
            callback(hookMessage);

            // !CHANGED! This failed beacuse you waited on the reset event for the CPPCallback receival
            Assert.IsTrue(consumedEvent.WaitOne(maxWaitTime), "Did not find a matching mouse event in time.");
        }

        /// <summary>
        /// !CHANGED! I altered the method to allow any mouse producer.
        /// It also uses a predicate to determine the event was received.
        /// </summary>
        /// <typeparam name="T">The type of event which will be captured.</typeparam>
        /// <param name="producer">The producer which offers the events.</param>
        /// <param name="reset">A reset event to identify if the predicate was met.</param>
        /// <param name="predicate">A predicate which should return true if the expected event has been received.</param>
        private async void findMatch<T>(DefaultEventQueue<T> producer, ManualResetEvent reset, Func<T, bool> predicate) where T : MouseEvent
        { 
            await foreach (var @event in producer.GetEvents())
            {
                if(predicate.Invoke(@event))
                {
                    reset.Set();
                    break;
                }
            }
        }
    }
}