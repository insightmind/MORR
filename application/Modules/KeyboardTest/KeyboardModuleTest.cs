using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.Keyboard;
using MORR.Modules.Keyboard.Events;
using MORR.Modules.Keyboard.Native;
using MORR.Modules.Keyboard.Producers;
using MORR.Shared.Hook;
using SharedTest.TestHelpers.INativeHook;
using System.Windows.Input;
using System.Threading.Tasks;
using MORR.Shared.Events.Queue;

namespace KeyboardTest
{
    [TestClass]
    public class KeyboardModuleTest
    {
        protected const int maxWaitTime = 5000;

        private CompositionContainer container;
        private KeyboardInteractEventProducer keyboardInteractEventProducer;
        private KeyboardModule keyboardModule;
        private Mock<INativeKeyboard> nativeKeyboardMock;
        private HookNativeMethodsMock hookNativeMethodsMock;

        private readonly GlobalHook.MessageType[] KeyboardInteractListenedMessagesTypes =
        {
            GlobalHook.MessageType.WM_KEYDOWN,
            GlobalHook.MessageType.WM_SYSKEYDOWN
        };

        [TestInitialize]
        public void BeforeTest()
        {
            //initialize module, producers and configuration
            keyboardModule = new KeyboardModule();
            keyboardInteractEventProducer = new KeyboardInteractEventProducer();

            // initialize the container and fulfill the MEF inports exports
            container = new CompositionContainer();
            container.ComposeExportedValue(keyboardInteractEventProducer);
            container.ComposeParts(keyboardModule);

            //initialize the native keyboard mock
            nativeKeyboardMock = new Mock<INativeKeyboard>();

            //initialzie the hookNativeMethodsMock
            hookNativeMethodsMock = new HookNativeMethodsMock();
            hookNativeMethodsMock.Initialize();
        }

        [TestMethod]
        public void TestKeyboardModule_ActivateTrue()
        {
            // Preconditions
            Debug.Assert(keyboardModule != null);

            /* GIVEN */

            /* WHEN */
            keyboardModule.Initialize(true);
            AllowMessageTypeRegistryForAll();
            hookNativeMethodsMock.AllowLibraryLoad();
            keyboardModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(keyboardModule.IsActive);
        }

        [TestMethod]
        public void TestKeyboardModule_ActivateFalse()
        {
            // Preconditions
            Debug.Assert(keyboardModule != null);

            /* GIVEN */

            /* WHEN */
            keyboardModule.Initialize(true);
            AllowMessageTypeRegistryForAll();
            hookNativeMethodsMock.AllowLibraryLoad();
            keyboardModule.IsActive = true;
            keyboardModule.IsActive = false;

            /* THEN */
            Assert.IsFalse(keyboardModule.IsActive);
        }

        [TestMethod]
        public void TestKeyboardModule_InitializeFalse()
        {
            // Preconditions
            Debug.Assert(keyboardModule != null);

            /* GIVEN */

            /* WHEN */
            keyboardModule.Initialize(false);
            /* THEN */
            Assert.IsTrue(keyboardInteractEventProducer.IsClosed);
        }

        [TestMethod]
        public void TestKeyboardModuleInitializeTrue()
        {
            // Preconditions
            Debug.Assert(keyboardModule != null);

            /* GIVEN */

            /* WHEN */
            keyboardModule.Initialize(true);

            /* THEN */
            Assert.IsFalse(keyboardInteractEventProducer.IsClosed);
        }

        [TestMethod]
        public void TestKeyboardInteractEventProducer_CallbackWithLetterNumberSymbols()
        {
            /* PRECONDITIONS */
            Debug.Assert(keyboardModule != null);
            Debug.Assert(keyboardInteractEventProducer != null);
            Debug.Assert(hookNativeMethodsMock != null);
            Debug.Assert(hookNativeMethodsMock.Mock != null);

            /* GIVEN */
            GlobalHook.CppGetMessageCallback callback = GetCallback(false);
            //setting up fake messages and corresponding expected Events
            GlobalHook.HookMessage[] hookMessages = {
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x41}, //A
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_SYSKEYDOWN, wParam = (IntPtr)0x48}, //H
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x36}, //6
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x30}, //0
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_SYSKEYDOWN, wParam = (IntPtr)0xBB}, //+
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0xBF}, //"/" or ?
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_SYSKEYDOWN, wParam = (IntPtr)0xA0},//Left SHIFT
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_SYSKEYDOWN, wParam = (IntPtr)0x24},//HOME
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x25}, //LeftArrow
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x12} //Alt
            };
            KeyboardInteractEvent[] expectedEvents = {
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.A},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.H},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.D6},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.D0},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.OemPlus},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.OemQuestion},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.LeftShift},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.Home},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.Left},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.LeftAlt}
            };

            /* WHEN */
            //Running the task in another thread
            var consumedEvent = new CountdownEvent(hookMessages.Length);
            var task = new Task(() => findMatch(keyboardInteractEventProducer, consumedEvent, expectedEvents, isKeyboardInteractEventFound));
            task.Start();
            // We must call the callback after we start the consumer for the producer.
            // otherwise the message is automatically dismissed.
            foreach (GlobalHook.HookMessage message in hookMessages)
            {
                callback(message);
            }

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(maxWaitTime), "Did not find all matching keyboard interact events in time.");
        }

        [TestMethod]
        public void TestKeyboardInteractEventProducer_CallbackWithModifierKeys()
        {
            /* PRECONDITIONS */
            Debug.Assert(keyboardModule != null);
            Debug.Assert(keyboardInteractEventProducer != null);
            Debug.Assert(hookNativeMethodsMock != null);
            Debug.Assert(hookNativeMethodsMock.Mock != null);
            Debug.Assert(nativeKeyboardMock != null);

            /* GIVEN */
            // Setting up the modifierkeys sequence which should work together with the fake messages to generate expected events
            nativeKeyboardMock.SetupSequence(nM => nM.IsKeyPressed(INativeKeyboard.VirtualKeyCode.VK_MENU))
               .Returns(true).Returns(false).Returns(false).Returns(false).Returns(false).Returns(true).Returns(true).Returns(false).Returns(true);
           

            nativeKeyboardMock.SetupSequence(nM => nM.IsKeyPressed(INativeKeyboard.VirtualKeyCode.VK_CONTROL))
               .Returns(false).Returns(true).Returns(false).Returns(false).Returns(false).Returns(false).Returns(true).Returns(true).Returns(true);
               

            nativeKeyboardMock.SetupSequence(nM => nM.IsKeyPressed(INativeKeyboard.VirtualKeyCode.VK_SHIFT))
               .Returns(false).Returns(false).Returns(true).Returns(false).Returns(false).Returns(true).Returns(false).Returns(true).Returns(true);

            nativeKeyboardMock.SetupSequence(nM => nM.IsKeyPressed(INativeKeyboard.VirtualKeyCode.VK_LWIN))
               .Returns(false).Returns(false).Returns(false).Returns(true).Returns(false).Returns(false).Returns(false).Returns(false).Returns(false);

            nativeKeyboardMock.Setup(nM => nM.IsKeyPressed(INativeKeyboard.VirtualKeyCode.VK_RWIN))
               .Returns(false);

            //setting up fake messages and corresponding expected Events
            GlobalHook.HookMessage[] hookMessages = {
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x41}, //A with Alt down
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x42}, //B with Control down
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x43}, //C with Shift down
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x44}, //D with Left Windows down
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x46}, //F with None
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x47}, //G with Alt + Shift down
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x48}, //H with Alt + Control down
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x49}, //I with Control + Shift down
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x4A} //J with Control + Alt + Shift Windows down
            };

            KeyboardInteractEvent[] expectedEvents = {
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.A, ModifierKeys_System_Windows_Input_ModifierKeys = ModifierKeys.Alt},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.B, ModifierKeys_System_Windows_Input_ModifierKeys = ModifierKeys.Control},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.C, ModifierKeys_System_Windows_Input_ModifierKeys = ModifierKeys.Shift},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.D, ModifierKeys_System_Windows_Input_ModifierKeys = ModifierKeys.Windows},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.F, ModifierKeys_System_Windows_Input_ModifierKeys = ModifierKeys.None},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.G, ModifierKeys_System_Windows_Input_ModifierKeys = ModifierKeys.Alt | ModifierKeys.Shift},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.H, ModifierKeys_System_Windows_Input_ModifierKeys = ModifierKeys.Alt | ModifierKeys.Control},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.I, ModifierKeys_System_Windows_Input_ModifierKeys = ModifierKeys.Shift | ModifierKeys.Control},
            new KeyboardInteractEvent { PressedKey_System_Windows_Input_Key = Key.J, ModifierKeys_System_Windows_Input_ModifierKeys = ModifierKeys.Alt | ModifierKeys.Control | ModifierKeys.Shift}
            };

            GlobalHook.CppGetMessageCallback callback = GetCallback(true);

            /* WHEN */
            //Running the task in another thread
            var consumedEvent = new CountdownEvent(hookMessages.Length);
            var task = new Task(() => findMatch(keyboardInteractEventProducer, consumedEvent, expectedEvents, isKeyboardInteractEventWithModifierKeysFound));
            task.Start();
            // We must call the callback after we start the consumer for the producer.
            // otherwise the message is automatically dismissed.
            foreach (GlobalHook.HookMessage message in hookMessages)
            {
                callback(message);
            }

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(maxWaitTime), "Did not find all matching keyboard interact events in time.");
        }


        /// <summary>
        ///     See if all the events generated by the producer machtes the expectation
        /// </summary>
        /// <typeparam name="T">The type of event which will be captured.</typeparam>
        /// <param name="producer">The producer which generates the events.</param>
        /// <param name="countdown">A reset event to identify if all the predicate was met.param>
        /// <param name="expectedEvents">an array of events that is expected to be generated by the producer</param>
        /// <param name="predicate">A predicate which should return true if the expected event has been received.</param>
        private async void findMatch<T>(DefaultEventQueue<T> producer, CountdownEvent countdown, T[] expectedEvents, Func<T, T[], bool> predicate) where T : KeyboardEvent
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
        ///     Call AllowMessageTypeRegistry() methods on all messages related to keyboard producers.
        /// </summary>
        private void AllowMessageTypeRegistryForAll()
        {
            foreach (GlobalHook.MessageType messageType in KeyboardInteractListenedMessagesTypes)
            {
                hookNativeMethodsMock.AllowMessageTypeRegistry(messageType);
            }
        }

        /// <summary>
        ///     Performs a series of initialization and Setups to get the CppGetMessageCallback.
        /// </summary>
        /// <param name="withNativeKeyboardMock">true if a nativeKeyboard mock is wanted, false if a nativeKeyboard is wanted</param>
        /// <returns>the callback that can be called with a message, which in turns calls a callback in the producers that is interested in this type of message</returns>
        private GlobalHook.CppGetMessageCallback GetCallback(bool withNativeKeyboardMock)
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
            keyboardModule.Initialize(true);

            if (withNativeKeyboardMock) { keyboardInteractEventProducer.StartCapture(nativeKeyboardMock.Object); }
            else { keyboardModule.IsActive = true; }

            //wait for the hookNativeMethodsMock.Mock.Callback is called!
            Assert.IsTrue(callbackReceivedEvent.WaitOne(maxWaitTime), "Did not receive callback in time!");
            callbackReceivedEvent.Dispose();
            Assert.IsNotNull(callback, "Callback received however unexpectedly null!");
            return callback;
        }

        /// <summary>
        ///     Return true if an keyboard Interact event is in the array of expected events.
        ///     This method itslef defines the logic to determine if two keyboardInteract events are equal.
        /// </summary>
        /// <param name="event">an event that is expected to be in the array of expected events</param>
        /// <param name="expectedEvents">an array of expected events</param>
        /// <returns>true if an event is indeed in the array of expected events.</returns>
        private bool isKeyboardInteractEventFound(KeyboardInteractEvent @event, KeyboardInteractEvent[] expectedEvents)
        {
            foreach (KeyboardInteractEvent e in expectedEvents)
            {
                if (@event.PressedKey_System_Windows_Input_Key.Equals(e.PressedKey_System_Windows_Input_Key)) return true;
            }
            return false;
        }

        /// <summary>
        ///     Return true if an keyboard Interact event is in the array of expected events.
        ///     This method itslef defines the logic to determine if two keyboardInteract events are equal.
        /// </summary>
        /// <param name="event">an event that is expected to be in the array of expected events</param>
        /// <param name="expectedEvents">an array of expected events</param>
        /// <returns>true if an event is indeed in the array of expected events.</returns>
        private bool isKeyboardInteractEventWithModifierKeysFound(KeyboardInteractEvent @event, KeyboardInteractEvent[] expectedEvents)
        {
            foreach (KeyboardInteractEvent e in expectedEvents)
            {
                if (@event.PressedKey_System_Windows_Input_Key.Equals(e.PressedKey_System_Windows_Input_Key) 
                    && @event.ModifierKeys_System_Windows_Input_ModifierKeys.Equals(e.ModifierKeys_System_Windows_Input_ModifierKeys)) return true;
            }
            return false;
        }
    }
}