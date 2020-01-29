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

LRESULT CALLBACK GetMsgProc(int nCode, WPARAM wParam, LPARAM lParam)
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
    if (nCode >= 0 && nCode == HC_ACTION)
    {
        unsigned int incremented;
        unsigned int type = msg->message;
        if ((type >= WM_MOUSEMOVE && type <= WM_MOUSEHWHEEL) || WM_KEYDOWN) {
            incremented = InterlockedIncrement(&bufferIterator); //atomic increment. overflows should not matter as we modulo it anyways.
            lastMsg[incremented % BUFFERSIZE] = { msg->message, msg->hwnd, (UINT)msg->wParam, msg->pt };
            timeStamps[incremented % BUFFERSIZE] = msg->time;
        }
        else if ((type >= WM_CUT && type <= WM_CLEAR)) {
            incremented = InterlockedIncrement(&bufferIterator);
            lastMsg[incremented % BUFFERSIZE] = { msg->message, msg->hwnd, 0, msg->pt };
            timeStamps[incremented % BUFFERSIZE] = msg->time;
        }
        else
        {
            goto forwardEvent;
        }
        ReleaseSemaphore(semaphore, 1, NULL);
    }
forwardEvent:
    return CallNextHookEx(GetMessageHook, nCode, wParam, lParam);
    //passing this message to target application
}

/**
    Return if the message type is captured by this DLL.
    Should be kept up-to-date with the switch case in the procMessage function.
    For event type values, see https://wiki.winehq.org/List_Of_Windows_Messages
*/
bool IsCaptured(UINT type) {
    return (type == WM_KEYDOWN
        || (type >= WM_MOUSEMOVE && type <= WM_MOUSEWHEEL)
        || (type >= WM_CREATE && type <= WM_ACTIVATE)
        || (type >= WM_CUT && type <= WM_CLEAR)
        );
}

DLL void SetHook(WH_MessageCallBack progressCallback) {
    running = 1;
    bufferIterator = 0;
    globalCallback = progressCallback;
    if (GetMessageHook == NULL)
        GetMessageHook = SetWindowsHookEx(WH_GETMESSAGE, GetMsgProc, hInstHookDll, 0);
    //if (CallWndProcHook == NULL)
        //CallWndProcHook = SetWindowsHookEx(WH_GETMESSAGE, GetMsgProc, hInstHookDll, 0);
    dispatcherthread = std::thread(dispatchForever);
    dispatcherthread.detach();
    printf("GlobalHook: Hooked\n");
}

//remove the hook
DLL void RemoveHook()
{
    if (GetMessageHook != NULL)
        UnhookWindowsHookEx(GetMessageHook);
    //if (CallWndProcHook != NULL)
        //UnhookWindowsHookEx(CallWndProcHook);
    GetMessageHook = NULL;
    //CallWndProcHook = NULL;
    GetMessageHook = NULL;
    running = false;
    printf("GlobalHook: Unhooked\n");
}

/**
    This function runs in the AS of the MORR application.
*/
void dispatchForever() {
    unsigned int localBufferIterator = 0;
    unsigned int previous;
    semaphore = CreateSemaphore(NULL, 0, BUFFERSIZE, TEXT(SEMAPHORE_GUID));
    while (running) {
        {
            while (localBufferIterator == bufferIterator % BUFFERSIZE)
                WaitForSingleObject(semaphore, INFINITE);
            previous = localBufferIterator;
            localBufferIterator = ++localBufferIterator % BUFFERSIZE;
            if (timeStamps[localBufferIterator] == timeStamps[(previous)])
                continue;
            //printf("Dispatcher: message from %d, event number %d, iterator %d\n, globaliterator %d\n", lastMsg[localBufferIterator % BUFFERSIZE].Hwnd, lastMsg[localBufferIterator % BUFFERSIZE].Type, localBufferIterator, bufferIterator);
            //printf("Message: %d, point: %d,%d, keycode %d\n", lastMsg[localBufferIterator % BUFFERSIZE].Type,
                //lastMsg[localBufferIterator % BUFFERSIZE].CursorPosition.x, lastMsg[localBufferIterator % BUFFERSIZE].CursorPosition.y, lastMsg[localBufferIterator % BUFFERSIZE].wParam);
            globalCallback(lastMsg[localBufferIterator % BUFFERSIZE]);
        }
    }
}