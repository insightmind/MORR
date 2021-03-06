﻿using System;
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
using MORR.Modules.Keyboard.Native;
using System.Collections.Generic;

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
            //get the callback
            GlobalHook.CppGetMessageCallback callback = GetCallback();

            //setting up fake nativeKeyboardMock behaviors, messages and corresponding expected Events
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x41)).Returns(Key.A);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x42)).Returns(Key.B);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x43)).Returns(Key.C);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x44)).Returns(Key.D);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x46)).Returns(Key.F);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x47)).Returns(Key.G);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x48)).Returns(Key.H);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x49)).Returns(Key.I);
            nativeKeyboardMock.Setup(nativeK => nativeK.KeyFromVirtualKey(0x4A)).Returns(Key.J);

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
            //true if all events generated by the fake messages have meet expectation.
            Assert.IsTrue(consumedEvent.Wait(maxWaitTime), "Did not find all matching keyboard interact events in time.");

            //total shut down and resources release
            keyboardInteractEventProducer.StopCapture();
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
            keyboardInteractEventProducer.StartCapture(nativeKeyboardMock.Object);

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
                if (@event.PressedKey_System_Windows_Input_Key.Equals(e.PressedKey_System_Windows_Input_Key)
                    && @event.ModifierKeys_System_Windows_Input_ModifierKeys.Equals(e.ModifierKeys_System_Windows_Input_ModifierKeys)
                    && @event.PressedKeyName.Equals(e.PressedKey_System_Windows_Input_Key.ToString())
                    && @event.ModifierKeysName.Equals(e.ModifierKeys_System_Windows_Input_ModifierKeys.ToString())) return true;
            }
            return false;
        }
    }
}