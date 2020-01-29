using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MORR.Shared.Utility
{
    public static class GlobalHook
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct WM_Message
        {
            [MarshalAs(UnmanagedType.U4)]
            public uint type;
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr Hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public uint wParam;
            public Point CursorPosition;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            [MarshalAs(UnmanagedType.U4)]
            public int x;
            [MarshalAs(UnmanagedType.U4)]
            public int y;
        }

        private static bool hooked = false; //TODO: make nicer
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void CppGetMessageCallback(WM_Message message);
        [DllImport(@"HookLibrary64", CallingConvention=CallingConvention.Cdecl)]
        private static extern void SetHook([MarshalAs(UnmanagedType.FunctionPtr)] CppGetMessageCallback callbackPointer);
        [DllImport(@"HookLibrary64", CallingConvention = CallingConvention.Cdecl)]
        private static extern void RemoveHook();

        [DllImport(@"HookLibrary64", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool IsCaptured([MarshalAs(UnmanagedType.U4)] uint type);

        public static class WH_GetMessage
        {
            public delegate void WorkCompletedCallBack(WM_Message message);
            static Dictionary<NativeMethods.MessageType, List<WorkCompletedCallBack>> listeners = new Dictionary<NativeMethods.MessageType, List<WorkCompletedCallBack>>();
            public class Point //TODO: move somewhere else
            {
                public int x;
                public int y;
            }

            public static void addListener(WorkCompletedCallBack callback, NativeMethods.MessageType type) {
                if (!IsCaptured((uint)type))
                    throw new NotSupportedException(String.Format("GlobalHook currently does not support this message type ({0})", type));
                if (!listeners.ContainsKey(type))
                    listeners.Add(type, new List<WorkCompletedCallBack>());
                listeners[type].Add(callback);
            }
            public static void removeListener(WorkCompletedCallBack callback, NativeMethods.MessageType type)
            {
                listeners[type].Remove(callback);
                if (listeners[type].Count == 0)
                    listeners.Remove(type);
            }

            internal static CppGetMessageCallback cppCallback = (message) =>
            {
                trigger((NativeMethods.MessageType)message.type, message); //TODO: correctly parse point
            };

            public async static void trigger(NativeMethods.MessageType type, WM_Message message) //todo: hide
            {
                if (listeners.ContainsKey(type))
                {
                    foreach (WorkCompletedCallBack cb in listeners[type])
                        cb(message);
                }
            }
        }
        public static void Hook()
        {
            if (!hooked)
            {
                SetHook(WH_GetMessage.cppCallback);
                hooked = true;
            }
        }

        public static void UnHook()
        {
            if (hooked)
            {
                RemoveHook();
                hooked = false;
            }
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            if (hooked)
                UnHook();
        }

        static GlobalHook()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        }
    }
}
