using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using MORR.Shared.Utility.Exceptions;

namespace MORR.Shared.Utility
{
    public static class GlobalHook
    {
        /// <summary>
        ///     CallBack definition for addListener and removeListener methods.
        /// </summary>
        /// <param name="message">the  <see cref="HookMessage" /> to be passed to the callback.</param>
        public delegate void RetrieveMessageCallBack(HookMessage message);

        /// <summary>
        ///     If the GlobalHook is active or not. 
        /// </summary>
        public static bool IsActive
        {
            get => isActive;
            set
            {
                if(value)
                {
                    Hook();
                }
                else
                {
                    Unhook();
                }
            }
        }

        private const string hookLibName = @"HookLibrary64";
        private static bool isActive;
        private static IntPtr hookLibrary;
        //map message types to lists of interested listeners
        private static readonly Dictionary<NativeMethods.MessageType, List<RetrieveMessageCallBack>> listeners =
            new Dictionary<NativeMethods.MessageType, List<RetrieveMessageCallBack>>();

        //this callback will get called by the native DLL code
        private static readonly CppGetMessageCallback cppCallback = message => Trigger((NativeMethods.MessageType) message.Type, message);

        #region Imports

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);

        [DllImport(hookLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void
            SetHook([MarshalAs(UnmanagedType.FunctionPtr)] CppGetMessageCallback callbackPointer);

        [DllImport(hookLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RemoveHook();

        [DllImport(hookLibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Capture([MarshalAs(UnmanagedType.U4)] uint type);

        [DllImport(hookLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void StopCapture([MarshalAs(UnmanagedType.U4)] uint type);
        #endregion

        #region Public functions

        /// <summary>
        ///     Add a listener to multiple message types.
        /// </summary>
        /// <param name="callback">The <see cref="RetrieveMessageCallBack" /> function to be called on messages.</param>
        /// <param name="types">An array of the types to listen for.</param>
        public static void AddListener(RetrieveMessageCallBack callback, params NativeMethods.MessageType[] types)
        {
            if (types.Any(type => !Capture((uint)type)))
{
                throw new NotSupportedException();
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
        ///     Remove a listener from multiple message types.
        /// </summary>
        /// <param name="callback">The <see cref="RetrieveMessageCallBack" /> to remove.</param>
        /// <param name="types">An array of the types to remove the callback for.</param>
        public static void RemoveListener(RetrieveMessageCallBack callback, params NativeMethods.MessageType[] types)
        {
            foreach (var type in types)
            {
                if (listeners.ContainsKey(type))
                {
                    listeners[type].Remove(callback);
                    if (listeners[type].Any())
                    {
                        listeners.Remove(type);
                        StopCapture((uint)type);
                    }
                }
            }
        }

        /// <summary>
        ///     Free the loaded library.
        /// </summary>
        public static void FreeLibrary()
        {
            if (hookLibrary != IntPtr.Zero)
            {
                //the library is loaded twice. Once for the DLLImports above, once by LoadLibrary.
                //LoadLibrary is only used to safely get a handle to the library.
                FreeLibrary(hookLibrary);
                FreeLibrary(hookLibrary);
                hookLibrary = IntPtr.Zero;
            }
        }

        #endregion

        #region Private functions

        private static void Trigger(NativeMethods.MessageType type, HookMessage message)
        {
            if (listeners.ContainsKey(type))
            {
                foreach (var callback in listeners[type])
                {
                    callback(message);
                }
            }
        }

        private static void Hook()
        {
            if (!isActive)
            {
                if (hookLibrary == IntPtr.Zero)
                {
                    hookLibrary = LoadLibrary(hookLibName);
                    if (hookLibrary == IntPtr.Zero)
                        throw new HookLibraryException($"Error loading {hookLibName}");
                }
                SetHook(cppCallback);
                isActive = true;
            }
        }

        private static void Unhook()
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
        public struct HookMessage
        {
            [MarshalAs(UnmanagedType.U4)] public uint Type;
            [MarshalAs(UnmanagedType.SysInt)] public IntPtr Hwnd;
            [MarshalAs(UnmanagedType.SysInt)] public IntPtr wParam;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I4)]
            public int[] Data; //General purpose fields for message specific data
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void CppGetMessageCallback(HookMessage message);
        #endregion
    }
}