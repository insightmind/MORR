using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MORR.Modules.Mouse.Producers
{
    static class CommonMethods
    {
        /// <summary>
        ///     Set the hook for the Mouse.
        /// </summary>
        public static void HookMouse(IntPtr hook)
        {
            var currentProcess = Process.GetCurrentProcess();
            var currentModule = currentProcess.MainModule;
            var moduleName = currentModule.ModuleName;
            var moduleHandle = NativeMethods.GetModuleHandle(moduleName);
            hook = NativeMethods.SetWindowsHookEx(the hook type, the hook proc callback, moduleHandle,
            0);
        }

        /// <summary>
        ///     Release the hook for the keyboard.
        /// </summary>
        public static void UnhookKeyboard(IntPtr hook)
        {
            NativeMethods.UnhookWindowsHookEx(hook);
        }

    }
}
