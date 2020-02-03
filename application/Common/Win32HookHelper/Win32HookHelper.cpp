#include "Win32HookHelper.h"

HMODULE Win32HookHelper::hookLibrary = NULL;
pInitFunc Win32HookHelper::SetHook = NULL;
pVoidFunc Win32HookHelper::RemoveHook = NULL;
pBoolFunc Win32HookHelper::Capture = NULL;
HANDLE Win32HookHelper::sharedMemory = NULL;


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
    if (sharedMemory)
        CloseHandle(sharedMemory);
    sharedMemory = nullptr;
    hookLibrary = nullptr;
    SetHook = nullptr;
    RemoveHook = nullptr;
}

void __stdcall Win32HookHelper::callback(WM_Message message) {
    
}