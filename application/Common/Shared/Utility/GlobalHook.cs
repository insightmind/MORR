using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MORR.Shared.Utility
{
    public static class GlobalHook
    {
        /// <summary>
        ///     CallBack definition for addListener and removeListener methods.
        /// </summary>
        /// <param name="message">the  <see cref="WM_Message" /> to be passed to the callback.</param>
        public delegate void RetrieveMessageCallBack(WM_Message message);

        /// <summary>
        ///     If the GlobalHook is active or not. 
        /// </summary>
        public static bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, Hook, UnHook);
        }

        private static bool isActive;
        //map message types to lists of interested listeners
        private static readonly Dictionary<NativeMethods.MessageType, List<RetrieveMessageCallBack>> listeners =
            new Dictionary<NativeMethods.MessageType, List<RetrieveMessageCallBack>>();

        //this callback will get called by the native DLL code
        internal static CppGetMessageCallback cppCallback = message => Trigger((NativeMethods.MessageType) message.type, message);

        #region Imports

        [DllImport(@"HookLibrary64", CallingConvention = CallingConvention.Cdecl)]
        private static extern void
            SetHook([MarshalAs(UnmanagedType.FunctionPtr)] CppGetMessageCallback callbackPointer);

        [DllImport(@"HookLibrary64", CallingConvention = CallingConvention.Cdecl)]
        private static extern void RemoveHook();

        [DllImport(@"HookLibrary64", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool capture([MarshalAs(UnmanagedType.U4)] uint type);

        [DllImport(@"HookLibrary64", CallingConvention = CallingConvention.Cdecl)]
        private static extern void stopCapture([MarshalAs(UnmanagedType.U4)] uint type);
        #endregion

        #region Public functions

        /// <summary>
        ///     Add a listener for a single message type.
        /// </summary>
        /// <param name="callback">The  <see cref="RetrieveMessageCallBack" /> function to be called on messages.</param>
        /// <param name="type">The message type to listen for.</param>
        public static void addListener(RetrieveMessageCallBack callback, NativeMethods.MessageType type)
        {
            if (capture((uint)type))
            {
                if (!listeners.ContainsKey(type))
                {
                    listeners.Add(type, new List<RetrieveMessageCallBack>());
                }

                listeners[type].Add(callback);
            }
            else
            {
                throw new NotSupportedException(
                    $"GlobalHook currently does not support this message type ({type})");
            }
        }

        /// <summary>
        ///     Add a listener to multiple message types.
        /// </summary>
        /// <param name="callback">The <see cref="RetrieveMessageCallBack" /> function to be called on messages.</param>
        /// <param name="types">An array of the types to listen for.</param>
        public static void addListener(RetrieveMessageCallBack callback, NativeMethods.MessageType[] types)
        {
            foreach (var type in types)
            {
                if (!capture((uint) type))
                {
                    throw new NotSupportedException(
                        $"GlobalHook currently does not support this message type ({type})");
                }
            }

            foreach (var type in types)
            {
                if (!listeners.ContainsKey(type))
                {
                    listeners.Add(type, new List<RetrieveMessageCallBack>());
                }

                listeners[type].Add(callback);
            }
        }

        /// <summary>
        ///     Remove a listener from a single message type.
        /// </summary>
        /// <param name="callback">The  <see cref="RetrieveMessageCallBack" /> to remove.</param>
        /// <param name="type">The message type to remove the callback for.</param>
        public static void RemoveListener(RetrieveMessageCallBack callback, NativeMethods.MessageType type)
        {
            if (listeners.ContainsKey(type))
            {
                listeners[type].Remove(callback);
                if (listeners[type].Count == 0)
                {
                    listeners.Remove(type);
                    stopCapture((uint) type);
                }
            }
        }

        /// <summary>
        ///     Remove a listener from multiple message types.
        /// </summary>
        /// <param name="callback">The <see cref="RetrieveMessageCallBack" /> to remove.</param>
        /// <param name="types">An array of the types to remove the callback for.</param>
        public static void RemoveListener(RetrieveMessageCallBack callback, NativeMethods.MessageType[] types)
        {
            foreach (var type in types)
            {
                if (listeners.ContainsKey(type))
                {
                    listeners[type].Remove(callback);
                    if (listeners[type].Count == 0)
                    {
                        listeners.Remove(type);
                        stopCapture((uint)type);
                    }
                }
            }
        }

        #endregion

        #region Private functions

        private static void Trigger(NativeMethods.MessageType type, WM_Message message) //todo: hide
        {
            if (listeners.ContainsKey(type))
            {
                foreach (var cb in listeners[type])
                {
                    cb(message);
                }
            }
        }

        private static void Hook()
        {
            if (!isActive)
            {
                SetHook(cppCallback);
                isActive = true;
            }
        }

        private static void UnHook()
        {
            if (isActive)
            {
                RemoveHook();
                isActive = false;
            }
        }

        #endregion

        #region Definitions for communication with native code

        /// <summary>
        ///     A custom Message struct for all message types.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WM_Message
        {
            [MarshalAs(UnmanagedType.U4)] public uint type;
            [MarshalAs(UnmanagedType.SysInt)] public IntPtr Hwnd;
            [MarshalAs(UnmanagedType.U4)] public uint wParam; //Note: the wParam definition might differ from the official specs based on message type.
            public Point Position; //the semantics of Position differ based on message type (e.g. may be cursor or window position)
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            [MarshalAs(UnmanagedType.U4)] public int x;
            [MarshalAs(UnmanagedType.U4)] public int y;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void CppGetMessageCallback(WM_Message message);
        #endregion
    }
}