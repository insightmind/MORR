using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.Keyboard;
using MORR.Modules.Keyboard.Events;
using MORR.Modules.Keyboard.Producers;
using MORR.Shared.Hook;
using SharedTest.TestHelpers.INativeHook;
using System.Windows.Input;
using System.Threading.Tasks;
using MORR.Modules.Keyboard.Native;

namespace KeyboardTest
{
    [TestClass]
    public class KeyboardInteractEventProducerModifierKeysTest
    {
        protected const int maxWaitTime = 5000;

        private CompositionContainer container;
        private KeyboardInteractEventProducer keyboardInteractEventProducer;
        private KeyboardModule keyboardModule;
        private HookNativeMethodsMock hookNativeMethodsMock;
        private Mock<INativeKeyboard> nativeKeyboardMock;
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

        [TestCleanup]
        public void AfterTest()
        {
            // null everything!
            keyboardModule = null;
            keyboardInteractEventProducer = null;
            nativeKeyboardMock = null;
            container.Dispose();
            container = null;
            hookNativeMethodsMock = null;
        }

        [TestMethod]
        public void TestKeyboardInteractEventProducer_ModifierKeysTest()
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

            GlobalHook.CppGetMessageCallback callback = GetCallback();

            /* WHEN */
            //Running the task in another thread
            var consumedEvent = new CountdownEvent(hookMessages.Length);

            //////////////////////////////////////////////Task Run version
            //Task.Run(() => FindMatch(keyboardInteractEventProducer, consumedEvent, expectedEvents, IsKeyboardInteractEventFound));
            //////////////////////////////////////////////Task start version
            //var task = new Task(() => FindMatch(keyboardInteractEventProducer, consumedEvent, expectedEvents, IsKeyboardInteractEventFound));
            //task.Start();
            //////////////////////////////////////////////Thread start version
            //Thread findMatch = new Thread(() => FindMatch(keyboardInteractEventProducer, consumedEvent, expectedEvents, IsKeyboardInteractEventFound));
            //findMatch.Start();

            var thread = new Thread(async () =>
            {
                await foreach (var @event in keyboardInteractEventProducer.GetEvents())
                {
                    if (!IsKeyboardInteractEventFound(@event, expectedEvents))
                    {
                        continue;
                    }
                    consumedEvent.Signal();
                }
            });
            thread.Start();


            // We must call the callback after we start the consumer for the producer.
            // otherwise the message is automatically dismissed.
            foreach (GlobalHook.HookMessage message in hookMessages)
            {
                callback(message);
            }

            /* THEN */
            Assert.IsTrue(consumedEvent.Wait(maxWaitTime), "Did not find all matching keyboard interact events in time.");
            keyboardInteractEventProducer.StopCapture();
            keyboardModule.Initialize(false);
            consumedEvent.Dispose();
        }

        /// <summary>
        ///     See if all the events generated by the producer machtes the expectation
        /// </summary>
        /// <param name="producer">The producer which generates the events.</param>
        /// <param name="countdown">A reset event to identify if all the predicate was met.param>
        /// <param name="expectedEvents">an array of events that is expected to be generated by the producer</param>
        /// <param name="predicate">A predicate which should return true if the expected event has been received.</param>
        private static async void FindMatch(KeyboardInteractEventProducer producer, CountdownEvent countdown, KeyboardInteractEvent[] expectedEvents, Func<KeyboardInteractEvent, KeyboardInteractEvent[], bool> predicate)
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
            keyboardModule.Initialize(true);
            keyboardInteractEventProducer.StartCapture(nativeKeyboardMock.Object);

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
        private bool IsKeyboardInteractEventFound(KeyboardInteractEvent @event, KeyboardInteractEvent[] expectedEvents)
        {
            foreach (KeyboardInteractEvent e in expectedEvents)
            {
                if (@event.PressedKey_System_Windows_Input_Key.Equals(e.PressedKey_System_Windows_Input_Key)
                    && @event.ModifierKeys_System_Windows_Input_ModifierKeys.Equals(e.ModifierKeys_System_Windows_Input_ModifierKeys)
                    && @event.PressedKeyName.Equals(e.PressedKey_System_Windows_Input_Key.ToString())
                    && @event.ModifierKeysName.Equals(e.ModifierKeys_System_Windows_Input_ModifierKeys.ToString())) return true;
            }
            return false;
        }
    }
}