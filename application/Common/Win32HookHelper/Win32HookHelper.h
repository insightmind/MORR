#ifndef WIN32HOOKHELPER_H
#define WIN32HOOKHELPER_H

#include <windows.h>
#include <libloaderapi.h>
#include <stdio.h>
#include <tchar.h>

/**
    Copy of the struct definition found in HookLibrary.
 */
typedef struct {
    UINT64 Hwnd;
    UINT64 wParam;
    UINT32 Type;
    INT32 data[4];
} WM_Message;

typedef void(__cdecl* pVoidFunc)(void);
typedef void(__cdecl* pInitFunc)(void(__stdcall* pFunc)(WM_Message), bool);

class Win32HookHelper {
public:
    /**
        Attach the hook and block as long as GlobalHook is active.
     */
    static bool init();
    static void freeResources();
private:
    static HMODULE hookLibrary;
    static pInitFunc SetHook;
    static pVoidFunc RemoveHook;
    static void __stdcall callback(WM_Message message);
};

#endif