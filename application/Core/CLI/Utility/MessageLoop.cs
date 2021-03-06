﻿using MORR.Shared.Utility;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MORR.Core.CLI.Utility
{
    /// <summary>
    /// A MessageLoop instantiates a cancelable Win32 message loop.
    /// 
    /// Make sure to start the loop only on the main thread!
    /// Keep in mind that the message loop does block its calling thread, so you have to cancel it via a separate thread!
    /// </summary>
    public class MessageLoop : IMessageLoop
    {
        #region Properties

        /// <summary>
        /// Describes whether the MessageLoop is running or not.
        /// </summary>
        public bool IsRunning => loopThreadId.HasValue;

        /// <summary>
        /// This value is used to temporarily store the thread 
        /// </summary>
        private uint? loopThreadId;

        /// <summary>
        /// Registered message id for the cancel message.
        /// </summary>
        private readonly uint cancelMessageId;

        /// <summary>
        /// This is the message identifier for our custom cancel message,
        /// which is used to register a custom message for the MessageLoop.
        /// </summary>
        private const string cancelMessage = "MORR.LOOP.MESSAGE.CANCEL";

        /// <summary>
        /// Dll library name for user32 imports.
        /// </summary>
        private const string user32Dll = "user32.dll";

        /// <summary>
        /// Dll library name for kernel32 imports.
        /// </summary>
        private const string kernel32Dll = "kernel32.dll";

        #endregion

        #region LifeCycle
        /// <summary>
        /// Creates a new message loop object which can be used to start a new Win32 message loop.
        /// </summary>
        public MessageLoop()
        {
            cancelMessageId = RegisterWindowMessage(cancelMessage);
        }

        #endregion

        #region Win32 Message Loop

        /// <summary>
        ///     Runs a standard Win32 message loop
        ///     <remarks>
        ///         Intended for use in contexts where a Win32 message loop is required for Windows-Hooks and no such loop
        ///         already exists (e.g. ConsoleApp).
        ///     </remarks>
        /// </summary>
        public void Start()
        {
            // We need to store the current thread id to later
            // be able to post the cancel message to the loop.
            loopThreadId = GetCurrentThreadId();

            int status;
            while ((status = GetMessage(out var msg, IntPtr.Zero, 0, 0)) != 0)
            {
                // -1 indicates error - do not process such messages
                if (status == -1) continue;

                var msgId = msg.Message;

                TranslateMessage(ref msg);
                DispatchMessage(ref msg);

                if (msgId == cancelMessageId) break;
            }
        }

        /// <summary>
        /// Stops a currently running message loop.
        /// </summary>
        public void Stop()
        {
            if (!loopThreadId.HasValue) return;
            PostThreadMessage(loopThreadId.Value, cancelMessageId, UIntPtr.Zero, IntPtr.Zero);
            loopThreadId = null;
        }

        #endregion

        #region NativeMethods

        [DllImport(user32Dll)]
        private static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport(user32Dll)]
        private static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport(user32Dll)]
        private static extern IntPtr DispatchMessage([In] ref MSG lpMsg);

        [DllImport(user32Dll, SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint RegisterWindowMessage(string lpString);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(user32Dll, SetLastError = true)]
        private static extern bool PostThreadMessage(uint threadId, uint msg, UIntPtr wParam, IntPtr lParam);

        [DllImport(kernel32Dll)]
        public static extern uint GetCurrentThreadId();

        #endregion

        #region Structs 

        public struct MSG
        {
            public IntPtr HWnd;
            public uint Message;
            public IntPtr WParam;
            public IntPtr LParam;
            public uint Time;
            public POINT Pt;
        }

        /// <summary>
        ///     The POINT is of two int(32 bits) for the usage in MSLLHOOKSTRUCT.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static implicit operator Point(POINT p)
            {
                return new Point(p.X, p.Y);
            }

            public static implicit operator POINT(Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        #endregion
    }
}
