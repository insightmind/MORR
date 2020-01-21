using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MORR.Modules.Mouse.Producers
{
    static class NativeMethods
    {
        #region #region Constant, Structure and Delegate Definitions
        public struct MouseHookStruct
        {
            POINT pt;
            int mouseData;
            int flags;
            int time;
            int dwExtraInfo;
        }

        public struct POINT
        {
            long x; //was LONG in the document
            long y;
        }

        public delegate int LowLevelMouseProc(int code, int wParam, ref MouseHookStruct lParam);


        #endregion



        #region DLL imports
        /// <summary>
        ///     Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
        /// </summary>
        /// <param name="idHook">The id of the event you want to hook</param>
        /// <param name="lpfn">The low level keyboard procedure callback.</param>
        /// <param name="hMod">The handle you want to attach the event to, can be null</param>
        /// <param name="dwThreadId">The thread you want to attach the event to, can be null</param>
        /// <returns>a handle to the desired hook</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook,
                                                     LowLevelMouseProc lpfn,
                                                     IntPtr hMod,
                                                     uint dwThreadId);

        /// <summary>
        ///     Unhooks the windows hook.
        /// </summary>
        /// <param name="hhk">The hook handle that was returned from SetWindowsHookEx</param>
        /// <returns>True if successful, false otherwise</returns>
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        /// <summary>
        ///     Calls the next hook.
        /// </summary>
        /// <param name="idHook">The hook id</param>
        /// <param name="nCode">The hook code</param>
        /// <param name="wParam">The wparam.</param>
        /// <param name="lParam">The lparam.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int
            CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref MouseHookStruct lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}
