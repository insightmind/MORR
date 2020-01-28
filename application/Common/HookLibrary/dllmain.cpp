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
    /*if (mtx == NULL) {
        mtx = CreateMutex(
            NULL,              // default security attributes
            FALSE,             // initially not owned
            TEXT(MUTEXNAME));
    } */

    //lParam contains pointer to MSG structure.
    msg = (MSG*)lParam;
    /* see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644981(v=vs.85) */
    if (nCode >= 0 && nCode == HC_ACTION && (msg->message == WM_CHAR))
    {

        /*DWORD waitresult = WaitForSingleObject(mtx, 100);
        if (waitresult != WAIT_OBJECT_0) {
            while (newMsg) {
                ReleaseMutex(mtx);
                while (newMsg)
                    continue;
                waitresult = WaitForSingleObject(mtx, 100);
                if (waitresult != WAIT_OBJECT_0)
                    goto forwardEvent;
                    } */
            counter++;
            lastMsg.hwnd = msg->hwnd;
            lastMsg.message = msg->message; //e. g. charcode in keyboard inputs
            lastMsg.point = msg->pt;
            newMsg = true;
            //ReleaseMutex(mtx);
    }
forwardEvent:
    return CallNextHookEx(hkKey, nCode, wParam, lParam);
    //passing this message to target application
}

DLL void SetHook(WH_MessageCallBack progressCallback) {
    counter = 0;
    mtx = CreateMutex(
        NULL,              // default security attributes
        FALSE,             // initially not owned
        TEXT(MUTEXNAME));
    globalCallback = progressCallback;
    if (hkKey == NULL)
        hkKey = SetWindowsHookEx(WH_GETMESSAGE, procCharMsg, hInstHookDll, 0);
    dispatcherthread = std::thread(dispatchForever);
    dispatcherthread.detach();
}

void dispatchForever() {
    while (true) {
        while (!newMsg) {
            continue;
        }
        //WaitForSingleObject(mtx, INFINITE);
        printf("Dispatcher: message from %d, event number %d\n", lastMsg.hwnd, counter);
        printf("Message: %d, point: %d,%d\n", lastMsg.message, lastMsg.point.x, lastMsg.point.y);
        globalCallback(lastMsg.hwnd, lastMsg.message, lastMsg.point); //currently crashes for some reason
        newMsg = 0;
        //ReleaseMutex(mtx);
    }
}