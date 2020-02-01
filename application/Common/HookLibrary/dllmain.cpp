// dllmain.cpp : Defines the entry point for the DLL application.

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
    LRESULT retVal = CallNextHookEx(GetMessageHook, nCode, wParam, lParam);
    //a pointer to hold the MSG structure that is passed as lParam
    MSG* msg;
    if (semaphore == NULL) {
        semaphore = CreateSemaphore(NULL, 0, BUFFERSIZE, TEXT(SEMAPHORE_GUID));
    }

    //lParam contains pointer to MSG structure.
    msg = (MSG*)lParam;
    /* see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644981(v=vs.85) */
    if (nCode >= 0 && nCode == HC_ACTION && msg->message < MESSAGETABLESIZE && messageHasListener[msg->message])
    {
        unsigned int bufferSlot = InterlockedIncrement(&globalBufferIterator) % BUFFERSIZE; //atomic increment. overflows should not matter as we modulo it anyways.
        unsigned int type = msg->message;
        globalTimeStamps[bufferSlot] = msg->time;
        if ((type >= WM_MOUSEMOVE && type <= WM_MOUSEWHEEL) || type == WM_KEYDOWN) {
            globalMessageBuffer[bufferSlot].Set(msg->message, msg->hwnd, msg->wParam);
            globalMessageBuffer[bufferSlot].data[0] = msg->pt.x;
            globalMessageBuffer[bufferSlot].data[1] = msg->pt.y;
        }
        else
        {
            /* this is only reached by implementation error */
            globalMessageBuffer[bufferSlot].Type == WM_NULL;
            goto forwardEvent;
        }
        ReleaseSemaphore(semaphore, 1, NULL); /* mark one message as available to the dispatcher running in the MORR AS */
    }
forwardEvent:
    /* pass message onto target application */
    return retVal;
}

LRESULT CALLBACK CallWndProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    LRESULT retVal = CallNextHookEx(CallWndProcHook, nCode, wParam, lParam);
    CWPSTRUCT* msg;
    if (semaphore == NULL) {
        semaphore = CreateSemaphore(NULL, 0, BUFFERSIZE, TEXT(SEMAPHORE_GUID));
    }

    msg = (CWPSTRUCT*)lParam;

    /* see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644981(v=vs.85) */
    if (nCode >= 0 && nCode == HC_ACTION && msg->message < MESSAGETABLESIZE && messageHasListener[msg->message])
    {
        unsigned int bufferSlot = InterlockedIncrement(&globalBufferIterator) % BUFFERSIZE;
        globalTimeStamps[bufferSlot] = 0;
        unsigned int type = msg->message;

        /* I did opt to go for if..elif instead of switch..case since some types can be checked by range */
        if (type == WM_CREATE)
        {
            globalMessageBuffer[bufferSlot] = { msg->message, msg->hwnd, 0, {((CREATESTRUCT*)msg->lParam)->x, ((CREATESTRUCT*)msg->lParam)->y } };
        }
        else if (type == WM_MOVE)
        {
            globalMessageBuffer[bufferSlot] = { msg->message, msg->hwnd, 0, {(int)(short)LOWORD(msg->lParam), (int)(short)HIWORD(msg->lParam)} };
        }
        else if (type == WM_DESTROY || type == WM_ACTIVATE || ((type >= WM_SETFOCUS) && (type <= WM_ENABLE)))
        {
            globalMessageBuffer[bufferSlot] = { msg->message, (type == WM_ACTIVATE) ? (HWND)msg->lParam : msg->hwnd, msg->wParam, {0, 0} };
        }
        else if (type == WM_SIZE) {
            globalMessageBuffer[bufferSlot] = { msg->message, msg->hwnd, msg->wParam,  {(int)(short)LOWORD(msg->lParam), (int)(short)HIWORD(msg->lParam)} };
        }
        else if (type == WM_SIZING)
        {
            globalMessageBuffer[bufferSlot] = { msg->message, msg->hwnd, msg->wParam,  0, 0 };
        }
        else if ((type >= WM_CUT && type <= WM_CLEAR)) {
            globalMessageBuffer[bufferSlot] = { msg->message, msg->hwnd, 0, 0 };
        }
        else
        {
            goto forwardEvent;
        }
        ReleaseSemaphore(semaphore, 1, NULL); /* mark one message as available to the dispatcher running in the MORR AS */
    }
forwardEvent:
    /* pass message onto target application */
    return retVal;
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

bool Capture(UINT type) {
    if (type < MESSAGETABLESIZE && IsCaptured(type)) {
        messageHasListener[type] = true;
        return true;
    }
    return false;
};

void StopCapture(UINT type) {
    if (type < MESSAGETABLESIZE)
        messageHasListener[type] = false;
};

DLL void SetHook(WH_MessageCallBack progressCallback) {
    running = true;
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
    dispatcherthread = new std::thread(DispatchLoop);
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
    CloseHandle(semaphore);
    semaphore = NULL;
    printf("GlobalHook: Unhooked\n");
}

void DispatchLoop() {
    unsigned int localBufferIterator = 0;
    unsigned int previous;
    semaphore = CreateSemaphore(NULL, 0, BUFFERSIZE, TEXT(SEMAPHORE_GUID));
    while (running) {
        {
            while (localBufferIterator == globalBufferIterator % BUFFERSIZE) {
                if (running)
                    WaitForSingleObject(semaphore, 1000); /* wake up every second so we could react to RemoveHook call.*/
                else
                    return;
            }
            previous = localBufferIterator;
            ++localBufferIterator %= BUFFERSIZE;
            if ((globalTimeStamps[localBufferIterator]) && ((globalTimeStamps[localBufferIterator]) == globalTimeStamps[(previous)])
                 && (globalMessageBuffer[localBufferIterator].Type == globalMessageBuffer[previous].Type)) /* discard event if it's obviously a duplicate (works only or events which
                                                                                                              inherently come with a timestamp) */
                continue;
            globalCallback(globalMessageBuffer[localBufferIterator % BUFFERSIZE]);
        }
    }
}

void WM_Message::Set(UINT32 type, HWND hwnd, WPARAM wParam)
{
    this->Type = type;
    this->Hwnd = hwnd;
    this->wParam = wParam;
    ZeroMemory(&this->data, sizeof(sizeof(this->data)));
}
