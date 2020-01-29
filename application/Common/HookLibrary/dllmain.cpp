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
        unsigned int bufferSlot;
        unsigned int type = msg->message;
        if ((type >= WM_MOUSEMOVE && type <= WM_MOUSEHWHEEL) || type == WM_KEYDOWN) {
            bufferSlot = InterlockedIncrement(&globalBufferIterator); //atomic increment. overflows should not matter as we modulo it anyways.
            globalMessageBuffer[bufferSlot % BUFFERSIZE] = { msg->message, msg->hwnd, msg->wParam, msg->pt };
            globalTimeStamps[bufferSlot % BUFFERSIZE] = msg->time;
        }
        else
        {
            goto forwardEvent;
        }
        ReleaseSemaphore(semaphore, 1, NULL); /* mark one message as available to the dispatcher running in the MORR AS */
    }
forwardEvent:
    /* pass message onto target application */
    return CallNextHookEx(GetMessageHook, nCode, wParam, lParam);
}

LRESULT CALLBACK CallWndProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    CWPSTRUCT* msg;
    if (semaphore == NULL) {
        semaphore = CreateSemaphore(NULL, 0, BUFFERSIZE, TEXT(SEMAPHORE_GUID));
    }

    msg = (CWPSTRUCT*)lParam;
    /* see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644981(v=vs.85) */
    if (nCode >= 0 && nCode == HC_ACTION)
    {
        unsigned int bufferSlot;
        unsigned int type = msg->message;
        /* I did opt to go for if..elif instead of switch..case since some types can be checked by range */
        if (type == WM_CREATE)
        {
            bufferSlot = InterlockedIncrement(&globalBufferIterator);
            globalMessageBuffer[bufferSlot % BUFFERSIZE] = { msg->message, msg->hwnd, 0, {((CREATESTRUCT*)msg->lParam)->x, ((CREATESTRUCT*)msg->lParam)->y } };
            globalTimeStamps[bufferSlot % BUFFERSIZE] = 0;
        }
        else if (type == WM_MOVE)
        {
            bufferSlot = InterlockedIncrement(&globalBufferIterator);
            globalMessageBuffer[bufferSlot % BUFFERSIZE] = { msg->message, msg->hwnd, 0, {(int)(short)LOWORD(msg->lParam), (int)(short)HIWORD(msg->lParam)} };
            globalTimeStamps[bufferSlot % BUFFERSIZE] = 0;
        }
        else if (type == WM_DESTROY || type == WM_ACTIVATE || ((type >= WM_SETFOCUS) && (type <= WM_ENABLE)))
        {
            bufferSlot = InterlockedIncrement(&globalBufferIterator);
            globalMessageBuffer[bufferSlot % BUFFERSIZE] = { msg->message, (type == WM_ACTIVATE) ? (HWND)msg->lParam : msg->hwnd, msg->wParam, {0, 0} };
            globalTimeStamps[bufferSlot % BUFFERSIZE] = 0;
        }
        else if (type == WM_SIZE) {
            bufferSlot = InterlockedIncrement(&globalBufferIterator);
            globalMessageBuffer[bufferSlot % BUFFERSIZE] = { msg->message, msg->hwnd, msg->wParam,  {(int)(short)LOWORD(msg->lParam), (int)(short)HIWORD(msg->lParam)} };
            globalTimeStamps[bufferSlot % BUFFERSIZE] = 0;
        }
        else if (type == WM_SIZING)
        {
            bufferSlot = InterlockedIncrement(&globalBufferIterator);
            globalMessageBuffer[bufferSlot % BUFFERSIZE] = { msg->message, msg->hwnd, msg->wParam,  0, 0 };
            globalTimeStamps[bufferSlot % BUFFERSIZE] = 0;
        }
        else if ((type >= WM_CUT && type <= WM_CLEAR)) {
            bufferSlot = InterlockedIncrement(&globalBufferIterator);
            globalMessageBuffer[bufferSlot % BUFFERSIZE] = { msg->message, msg->hwnd, 0, 0 };
            globalTimeStamps[bufferSlot % BUFFERSIZE] = 0;
        }
        else
        {
            goto forwardEvent;
        }
        ReleaseSemaphore(semaphore, 1, NULL); /* mark one message as available to the dispatcher running in the MORR AS */
    }
forwardEvent:
    /* pass message onto target application */
    return CallNextHookEx(GetMessageHook, nCode, wParam, lParam);
}


/**
    For event type values, see https://wiki.winehq.org/List_Of_Windows_Messages
*/
bool IsCaptured(UINT type) {
    return (type == WM_KEYDOWN
        || (type >= WM_MOUSEMOVE && type <= WM_MOUSEWHEEL)
        || (type >= WM_CREATE && type <= WM_ENABLE)
        || (type >= WM_CUT && type <= WM_CLEAR)
        || (type == WM_SIZING)
        );
}

DLL void SetHook(WH_MessageCallBack progressCallback) {
    running = 1;
    globalBufferIterator = 0;
    globalCallback = progressCallback;
    if (GetMessageHook == NULL) {
        if ((GetMessageHook = SetWindowsHookEx(WH_GETMESSAGE, GetMsgProc, hInstHookDll, 0)) == NULL)
            printf("Error attaching GetMessage hook. Errorcode %d\n", GetLastError());
    }
    if (CallWndProcHook == NULL) {
        if ((CallWndProcHook = SetWindowsHookEx(WH_CALLWNDPROC, CallWndProc, hInstHookDll, 0)) == NULL)
            printf("Error attaching WndProc hook. Errorcode %d\n", GetLastError());
    }
    if (GetMessageHook == NULL || CallWndProcHook == NULL) {
        RemoveHook();
        return;
    }
    dispatcherthread = new std::thread(dispatchForever);
    printf("GlobalHook: Hooked\n");
}

DLL void RemoveHook()
{
    if (GetMessageHook != NULL) {
        if (!UnhookWindowsHookEx(GetMessageHook))
            printf("Error unhooking GetMessage hook. Errorcode %d\n", GetLastError());
    }
    if (CallWndProcHook != NULL) {
        if (!UnhookWindowsHookEx(CallWndProcHook))
            printf("Error unhooking CallWndProc hook. Errorcode %d\n", GetLastError());
    }
    running = false;
    GetMessageHook = NULL;
    CallWndProcHook = NULL;
    GetMessageHook = NULL;
    dispatcherthread->join();
    delete dispatcherthread;
    dispatcherthread = NULL;
    printf("GlobalHook: Unhooked\n");
}

void dispatchForever() {
    unsigned int localBufferIterator = 0;
    unsigned int previous;
    semaphore = CreateSemaphore(NULL, 0, BUFFERSIZE, TEXT(SEMAPHORE_GUID));
    while (running) {
        {
            while (localBufferIterator == globalBufferIterator % BUFFERSIZE) {
                if (running)
                    WaitForSingleObject(semaphore, 1000); /* wake up every second so we could react to RemoveHook call.*/
                else
                    goto terminate;
            }

            previous = localBufferIterator;
            localBufferIterator = ++localBufferIterator % BUFFERSIZE;
            if ((globalTimeStamps[localBufferIterator] == globalTimeStamps[(previous)])
                 && (globalMessageBuffer[localBufferIterator].Type == globalMessageBuffer[previous].Type)) /* discard event if it's a duplicate */
                continue;
            globalCallback(globalMessageBuffer[localBufferIterator % BUFFERSIZE]);
        }
    }
terminate:
    /* nothing to see here, move on */
    (void)0;
}