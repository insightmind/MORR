// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include "dllmain.h"

BOOL APIENTRY DllMain(HANDLE hModule, DWORD  ul_reason_for_call,
    LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        
        hInstHookDll = (HINSTANCE)hModule;
        break;
    }
    return TRUE;
}

LRESULT CALLBACK procCharMsg(int nCode, WPARAM wParam, LPARAM lParam)
//this is the hook procedure
{
    //a pointer to hold the MSG structure that is passed as lParam
    MSG* msg;
    if (mtx == NULL) {
        mtx = CreateMutex(
            NULL,              // default security attributes
            FALSE,             // initially not owned
            TEXT(MUTEXNAME));
    }

    //lParam contains pointer to MSG structure.
    msg = (MSG*)lParam;
    /* see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644981(v=vs.85) */
    if (nCode >= 0 && nCode == HC_ACTION && msg->message == WM_KEYDOWN)
    {
        WaitForSingleObject(mtx, INFINITE);
        counter++;
        lastMsg = { msg->message, msg->hwnd, (UINT)msg->wParam, msg->pt };
        newMsg = true;
        ReleaseMutex(mtx);
    }
forwardEvent:
    return CallNextHookEx(hkKey, nCode, wParam, lParam);
    //passing this message to target application
}

DLL void SetHook(WH_MessageCallBack progressCallback) {
    counter = 0;
    globalCallback = progressCallback;
    if (hkKey == NULL)
        hkKey = SetWindowsHookEx(WH_GETMESSAGE, procCharMsg, hInstHookDll, 0);
    dispatcherthread = std::thread(dispatchForever);
    dispatcherthread.detach();
}

void dispatchForever() {
    /*mtx = CreateMutex(
        NULL,              // default security attributes
        FALSE,             // initially not owned
        TEXT(MUTEXNAME)); */
    while (true) {
        while (!newMsg) {
            
        }

        WaitForSingleObject(mtx, INFINITE);
        //printf("Dispatcher: message from %d, event number %d\n", lastMsg.Hwnd, lastMsg.Type);
        //printf("Message: %d, point: %d,%d, keycode %d\n", lastMsg.Type, lastMsg.CursorPosition.x, lastMsg.CursorPosition.y, lastMsg.wParam);
        globalCallback(lastMsg);
        newMsg = false;
        ReleaseMutex(mtx);
    }
}