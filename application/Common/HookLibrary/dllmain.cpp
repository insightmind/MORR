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
    if (semaphore == NULL) {
        semaphore = CreateSemaphore(NULL, 0, BUFFERSIZE, TEXT(SEMAPHORE_GUID));
    }

    //lParam contains pointer to MSG structure.
    msg = (MSG*)lParam;
    /* see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644981(v=vs.85) */
    if (nCode >= 0 && nCode == HC_ACTION && msg->message == WM_KEYDOWN)
    {
        unsigned int incremented = InterlockedIncrement(&bufferIterator);
        lastMsg[incremented % BUFFERSIZE] = { msg->message, msg->hwnd, (UINT)msg->wParam, msg->pt };
        timeStamps[incremented % BUFFERSIZE] = msg->time;
        ReleaseSemaphore(semaphore, 1, NULL);
    }
forwardEvent:
    return CallNextHookEx(hkKey, nCode, wParam, lParam);
    //passing this message to target application
}

DLL void SetHook(WH_MessageCallBack progressCallback) {
    running = 1;
    bufferIterator = 0;
    globalCallback = progressCallback;
    if (hkKey == NULL)
        hkKey = SetWindowsHookEx(WH_GETMESSAGE, procCharMsg, hInstHookDll, 0);
    dispatcherthread = std::thread(dispatchForever);
    dispatcherthread.detach();
    printf("GlobalHook: Hooked\n");
}

//remove the hook
DLL void RemoveHook()
{
    if (hkKey != NULL)
        UnhookWindowsHookEx(hkKey);
    hkKey = NULL;
    running = false;
    printf("GlobalHook: Unhooked\n");
}

void dispatchForever() {
    unsigned int localBufferIterator = 0;
    unsigned int previous;
    semaphore = CreateSemaphore(NULL, 0, BUFFERSIZE, TEXT(SEMAPHORE_GUID));
    while (running) {
        {
            while (localBufferIterator == bufferIterator)
                WaitForSingleObject(semaphore, INFINITE);
            previous = localBufferIterator;
            localBufferIterator = ++localBufferIterator % BUFFERSIZE;
            if (timeStamps[localBufferIterator] == timeStamps[(previous)])
                continue;
            printf("Dispatcher: message from %d, event number %d, iterator %d\n", lastMsg[localBufferIterator % BUFFERSIZE].Hwnd, lastMsg[localBufferIterator % BUFFERSIZE].Type, localBufferIterator);
            //printf("Message: %d, point: %d,%d, keycode %d\n", lastMsg.Type, lastMsg.CursorPosition.x, lastMsg.CursorPosition.y, lastMsg.wParam);
            globalCallback(lastMsg[localBufferIterator % BUFFERSIZE]);
        }
    }
}