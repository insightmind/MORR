using System;
using System.Runtime.InteropServices;

namespace MORR.Shared.Hook
{
    public interface INativeHook
    {
        string HookLibraryName { get; }

        IntPtr LoadLibrary();

        bool FreeLibrary(IntPtr hModule);
        void SetHook([MarshalAs(UnmanagedType.FunctionPtr)] GlobalHook.CppGetMessageCallback callbackPointer, [MarshalAs(UnmanagedType.Bool)] bool blocking);

        void RemoveHook();

        bool Capture([MarshalAs(UnmanagedType.U4)] uint type);

        void StopCapture([MarshalAs(UnmanagedType.U4)] uint type);
    }
}
