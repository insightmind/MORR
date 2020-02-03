#ifndef WIN32HOOKHELPER_H
#define WIN32HOOKHELPER_H

#include <windows.h>
#include <libloaderapi.h>
#include <stdio.h>
#include <tchar.h>

typedef void(__cdecl* pVoidFunc)(void);
typedef bool(__cdecl* pBoolFunc)(UINT);
typedef int(__cdecl* pIntFunc)(void);
typedef struct {
    INT64 Hwnd;
    INT64 wParam;
    UINT32 Type;
    INT32 data[4];
} WM_Message;

typedef void(__cdecl* pInitFunc)(void(__stdcall* pFunc)(WM_Message), bool);

class Win32HookHelper {
public:
    static bool init();
    static void freeResources();
private:
    static HMODULE hookLibrary;
    static pInitFunc SetHook;
    static pVoidFunc RemoveHook;
    static pBoolFunc Capture;
    static HANDLE sharedMemory;
    static void __stdcall callback(WM_Message message);
};

#endif