using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MORR.Shared.Utility
{
    public static class GlobalHook
    {
        private static bool hooked = false; //TODO: make nicer
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void CppGetMessageCallback(IntPtr hwnd, uint message, UInt64 point);
        /*[DllImport(@"HookLibrary64")]
        private static extern void SetHook([MarshalAs(UnmanagedType.FunctionPtr)] CppGetMessageCallback callbackPointer);
        [DllImport(@"HookLibrary64")]
        private static extern void RemoveHook(); */
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetHook([MarshalAs(UnmanagedType.FunctionPtr)] CppGetMessageCallback callbackPointer);

        public static class WH_GetMessage
        {
            public delegate void WorkCompletedCallBack(IntPtr hwnd, uint message, Point result);
            static HashSet<WorkCompletedCallBack> listeners = new HashSet<WorkCompletedCallBack>();
            public class Point //TODO: move somewhere else
            {
                public int x;
                public int y;
            }

            public static void addListener(WorkCompletedCallBack callback) {
                listeners.Add(callback);
            }
            public static void removeListener(WorkCompletedCallBack callback)
            {
                listeners.Remove(callback);
            }

            internal static CppGetMessageCallback cppCallback = (hwnd, message, point) =>
            {
                trigger(hwnd, message, new Point { x = (int)point }); //TODO: correctly parse point
            };

            public async static void trigger(IntPtr hwnd, uint message, Point result) //todo: hide
            {
                foreach (WorkCompletedCallBack cb in listeners)
                    cb(hwnd, message, result);
            }
        }
        public static void Hook()
        {
            IntPtr pDll = NativeMethods.LoadLibrary(@"HookLibrary64.DLL");
            IntPtr pAddressOfFunctionToCall1 = NativeMethods.GetProcAddress(pDll, "SetHook");
            SetHook setHook = (SetHook)Marshal.GetDelegateForFunctionPointer(
                pAddressOfFunctionToCall1,
                typeof(SetHook));
            if (!hooked)
            {
                setHook(WH_GetMessage.cppCallback);
                hooked = true;
            }
        }

        public static void UnHook()
        {
            throw new NotImplementedException("Method not implemented");
            if (hooked)
            {
                //RemoveHook();
                hooked = false;
            }
        }
    }
}
