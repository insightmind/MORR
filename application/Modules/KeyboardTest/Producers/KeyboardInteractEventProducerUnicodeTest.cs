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
using System.Collections.Generic;

namespace KeyboardTest
{
    [TestClass]
    public class KeyboardInteractEventProducerUnicodeTest
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
        public void TestKeyboardInteractEventProducer_UnicdoeTest()
        {
            /* PRECONDITIONS */
            Debug.Assert(keyboardModule != null);
            Debug.Assert(keyboardInteractEventProducer != null);
            Debug.Assert(hookNativeMethodsMock != null);
            Debug.Assert(hookNativeMethodsMock.Mock != null);

            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x41)).Returns(Key.A);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x5A)).Returns(Key.Z);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x30)).Returns(Key.D0);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x39)).Returns(Key.D9);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0xBD)).Returns(Key.OemMinus);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0xBF)).Returns(Key.OemQuestion);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x12)).Returns(Key.LeftAlt);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x11)).Returns(Key.LeftShift);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x20)).Returns(Key.Space);

            nativeKeyboardMock.Setup(nativeK => nativeK.ToUnicode(0x41)).Returns('a');
            nativeKeyboardMock.Setup(nativeK => nativeK.ToUnicode(0x5A)).Returns('z');
            nativeKeyboardMock.Setup(nativeK => nativeK.ToUnicode(0x30)).Returns('0');
            nativeKeyboardMock.Setup(nativeK => nativeK.ToUnicode(0x39)).Returns('9');
            nativeKeyboardMock.Setup(nativeK => nativeK.ToUnicode(0xBD)).Returns('-');
            nativeKeyboardMock.Setup(nativeK => nativeK.ToUnicode(0xBF)).Returns('/');
            nativeKeyboardMock.Setup(nativeK => nativeK.ToUnicode(0x12)).Returns('\u0000');
            nativeKeyboardMock.Setup(nativeK => nativeK.ToUnicode(0x11)).Returns('\u0000');
            nativeKeyboardMock.Setup(nativeK => nativeK.ToUnicode(0x20)).Returns(' ');

            /* GIVEN */
            GlobalHook.CppGetMessageCallback callback = GetCallback();
            //setting up fake messages and corresponding expected Events
            GlobalHook.HookMessage[] hookMessages = {
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x41}, //a
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x5A}, //z
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x30}, //0
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x39}, //9
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_SYSKEYDOWN, wParam = (IntPtr)0xBD},//-
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0xBF}, // '/'
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x12}, //alt
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x11}, //control
            new GlobalHook.HookMessage { Type = (uint)GlobalHook.MessageType.WM_KEYDOWN, wParam = (IntPtr)0x20} //space
            
            };
            KeyboardInteractEvent[] expectedEvents = {
            new KeyboardInteractEvent {PressedKey_System_Windows_Input_Key = Key.A ,MappedCharacter_Unicode = 'a'},
            new KeyboardInteractEvent {PressedKey_System_Windows_Input_Key = Key.Z ,MappedCharacter_Unicode = 'z'},
            new KeyboardInteractEvent {PressedKey_System_Windows_Input_Key = Key.D0 ,MappedCharacter_Unicode = '0'},
            new KeyboardInteractEvent {PressedKey_System_Windows_Input_Key = Key.D9 ,MappedCharacter_Unicode = '9'},
            new KeyboardInteractEvent {PressedKey_System_Windows_Input_Key = Key.OemMinus ,MappedCharacter_Unicode = '-'},
            new KeyboardInteractEvent {PressedKey_System_Windows_Input_Key = Key.OemQuestion ,MappedCharacter_Unicode = '/'},
            new KeyboardInteractEvent {PressedKey_System_Windows_Input_Key = Key.LeftAlt ,MappedCharacter_Unicode = '\u0000'},
            new KeyboardInteractEvent {PressedKey_System_Windows_Input_Key = Key.LeftCtrl ,MappedCharacter_Unicode = '\u0000'},
            new KeyboardInteractEvent {PressedKey_System_Windows_Input_Key = Key.Space ,MappedCharacter_Unicode = ' '}
            };

            Assert.IsTrue(hookMessages.Length == expectedEvents.Length);

            /* WHEN */
            /* WHEN */
            // consumedEvent.Signal() will be called gdw one event has been found and meet expectation
            using var consumedEvent = new CountdownEvent(hookMessages.Length);
            Assert.IsTrue(consumedEvent.CurrentCount == hookMessages.Length);

            //didStartConsumingEvent.Set() will be called in Await method gdw the consumer is attached
            using var didStartConsumingEvent = new ManualResetEvent(false);

            //Running the task in another thread
            var thread = new Thread(async () =>
            {
                await foreach (var @event in Await<IAsyncEnumerable<KeyboardInteractEvent>>(keyboardInteractEventProducer.GetEvents(), didStartConsumingEvent))
                {
                    if (!IsKeyboardInteractEventFound(@event, expectedEvents))
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
            Assert.IsTrue(consumedEvent.Wait(maxWaitTime), "Did not find all matching keyboard interact events in time.");

            //total shut down and resources release
            keyboardModule.IsActive = false;
            keyboardModule.Initialize(false);
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
            using var callbackReceivedEvent = new AutoResetEvent(false);

            hookNativeMethodsMock.Mock
                 .Setup(hook => hook.SetHook(It.IsAny<GlobalHook.CppGetMessageCallback>(), It.IsAny<bool>()))?
                 .Callback((GlobalHook.CppGetMessageCallback cppCallback, bool isBlocking) =>
                 {
                     callback = cppCallback;
                     callbackReceivedEvent.Set();
                 });
            //here the SetHook() method is called!
            keyboardModule.Initialize(true);
            keyboardModule.IsActive = true;

            //wait for the hookNativeMethodsMock.Mock.Callback is called!
            Assert.IsTrue(callbackReceivedEvent.WaitOne(maxWaitTime), "Did not receive callback in time!");
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
                if ((@event.PressedKey_System_Windows_Input_Key.Equals(e.PressedKey_System_Windows_Input_Key)
                    && @event.PressedKey_System_Windows_Input_Key.ToString().Equals(e.PressedKey_System_Windows_Input_Key.ToString()))
                    && @event.MappedCharacter_Unicode.Equals(e.MappedCharacter_Unicode)) return true;
            }
            return false;
        }
    }
}