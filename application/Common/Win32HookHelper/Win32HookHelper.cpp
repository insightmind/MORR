#include "Win32HookHelper.h"

HMODULE Win32HookHelper::hookLibrary = nullptr;
pInitFunc Win32HookHelper::SetHook = nullptr;
pVoidFunc Win32HookHelper::RemoveHook = nullptr;


bool Win32HookHelper::init() {
    hookLibrary = LoadLibrary(TEXT("HookLibrary32.dll"));
    if (hookLibrary == NULL)
        return false;
    SetHook = (pInitFunc)GetProcAddress(hookLibrary, "SetHook");
    RemoveHook = (pVoidFunc)GetProcAddress(hookLibrary, "RemoveHook");

    if (!SetHook || !RemoveHook) {
        FreeLibrary(hookLibrary);
        return false;
    }
    fprintf(stderr, "Attempting to set hook\n");
    SetHook(&callback, true);

    return true;
}

void Win32HookHelper::freeResources() {
    RemoveHook();
    if (hookLibrary)
        FreeLibrary(hookLibrary);
    hookLibrary = nullptr;
    SetHook = nullptr;
    RemoveHook = nullptr;
}

void __stdcall Win32HookHelper::callback(WM_Message message) {
    
}