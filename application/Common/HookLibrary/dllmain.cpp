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
    if (semaphore == nullptr) {
        semaphore = CreateSemaphore(nullptr, 0, BUFFERSIZE, TEXT(SEMAPHORE_GUID));
    }

    //lParam contains pointer to MSG structure.
    msg = (MSG*)lParam;
    /* see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644981(v=vs.85) */
    if (nCode >= 0 && nCode == HC_ACTION && msg->message < MESSAGETABLESIZE && messageHasListener[msg->message])
    {
        unsigned int bufferSlot = InterlockedIncrement(&globalBufferIterator) % BUFFERSIZE; //atomic increment. overflows should not matter as we modulo it anyways.
        unsigned int type = msg->message;
        globalTimeStamps[bufferSlot] = msg->time;
        if ((type >= WM_MOUSEMOVE && type <= WM_MOUSEWHEEL) || (type >= WM_NCMOUSEMOVE && type <= WM_NCMBUTTONDBLCLK)|| type == WM_KEYDOWN) {
            globalMessageBuffer[bufferSlot].Set(msg->message, msg->hwnd, msg->wParam);
            globalMessageBuffer[bufferSlot].data[0] = msg->pt.x;
            globalMessageBuffer[bufferSlot].data[1] = msg->pt.y;
        }
        else
        {
            globalMessageBuffer[bufferSlot].Type == WM_NULL;
            goto forwardEvent;
        }
        ReleaseSemaphore(semaphore, 1, nullptr); /* mark one message as available to the dispatcher running in the MORR AS */
    }
forwardEvent:
    /* pass message onto target application */
    return retVal;
}

LRESULT CALLBACK CallWndProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    LRESULT retVal = CallNextHookEx(CallWndProcHook, nCode, wParam, lParam);
    CWPSTRUCT* msg;
    if (semaphore == nullptr) {
        semaphore = CreateSemaphore(nullptr, 0, BUFFERSIZE, TEXT(SEMAPHORE_GUID));
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
            globalMessageBuffer[bufferSlot].Set(msg->message, msg->hwnd, 0);
            globalMessageBuffer[bufferSlot].data[0] = ((CREATESTRUCT*)msg->lParam)->x;
            globalMessageBuffer[bufferSlot].data[1] = ((CREATESTRUCT*)msg->lParam)->y;
        }
        else if (type == WM_MOVE)
        {
            globalMessageBuffer[bufferSlot].Set(msg->message, msg->hwnd, 0);
            globalMessageBuffer[bufferSlot].data[0] = (int)LOWORD(msg->lParam);
            globalMessageBuffer[bufferSlot].data[1] = (int)HIWORD(msg->lParam);
        }
        else if (type == WM_DESTROY || type == WM_ACTIVATE || ((type >= WM_SETFOCUS) && (type <= WM_ENABLE)))
        {
            globalMessageBuffer[bufferSlot].Set(msg->message, (type == WM_ACTIVATE) ? (HWND)msg->lParam : msg->hwnd, msg->wParam);
        }
        else if (type == WM_SIZE) {
            globalMessageBuffer[bufferSlot].Set(msg->message, msg->hwnd, msg->wParam);
            globalMessageBuffer[bufferSlot].data[0] = (int)LOWORD(msg->lParam);
            globalMessageBuffer[bufferSlot].data[1] = (int)HIWORD(msg->lParam);
        }
        else if (type == WM_SIZING)
        {
            globalMessageBuffer[bufferSlot].Set(msg->message, msg->hwnd, msg->wParam);
        }
        else if ((type >= WM_CUT && type <= WM_CLEAR)) {
            globalMessageBuffer[bufferSlot].Set(msg->message, msg->hwnd, msg->wParam);
        }
        else
        {
            globalMessageBuffer[bufferSlot].Type == WM_NULL;
            goto forwardEvent;
        }
        ReleaseSemaphore(semaphore, 1, nullptr); /* mark one message as available to the dispatcher running in the MORR AS */
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
        || (type >= WM_NCMOUSEMOVE && type <= WM_NCMBUTTONDBLCLK)
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
    if (GetMessageHook || CallWndProcHook) {
        /* the stored values in this DLL might be persisting even
           over program restarts if not detached properly */
        fprintf(stderr, "GlobalHook wasn't properly detached last time, trying to reset before attaching\n");
        RemoveHook();
    }
    running = true;
    globalBufferIterator = 0;
    globalCallback = progressCallback;

    if (GetMessageHook == nullptr) {
        if ((GetMessageHook = SetWindowsHookEx(WH_GETMESSAGE, GetMsgProc, hInstHookDll, 0)) == nullptr)
            fprintf(stderr, "Error attaching GetMessage hook. Errorcode %d\n", GetLastError());
    }
    if (CallWndProcHook == nullptr) {
        if ((CallWndProcHook = SetWindowsHookEx(WH_CALLWNDPROC, CallWndProc, hInstHookDll, 0)) == nullptr)
            fprintf(stderr, "Error attaching WndProc hook. Errorcode %d\n", GetLastError());
    }
    if (GetMessageHook == nullptr || CallWndProcHook == nullptr) {
        RemoveHook();
        return;
    }
    dispatcherthread = new std::thread(DispatchLoop);
    printf("GlobalHook: Hooked\n");
}

DLL void RemoveHook()
{
    running = false;
    if (GetMessageHook != nullptr) {
        if (!UnhookWindowsHookEx(GetMessageHook))
            fprintf(stderr, "Error unhooking GetMessage hook. Errorcode %d\n", GetLastError());
    }
    if (CallWndProcHook != nullptr) {
        if (!UnhookWindowsHookEx(CallWndProcHook))
            fprintf(stderr, "Error unhooking CallWndProc hook. Errorcode %d\n", GetLastError());
    }
    GetMessageHook = nullptr;
    CallWndProcHook = nullptr;
    GetMessageHook = nullptr;
    if (dispatcherthread) {
        dispatcherthread->join();
        delete dispatcherthread;
    }
    dispatcherthread = nullptr;
    if (semaphore)
        CloseHandle(semaphore);
    semaphore = nullptr;
    globalBufferIterator = 0;
    ZeroMemory(globalMessageBuffer, sizeof(globalMessageBuffer));
    printf("GlobalHook: Unhooked\n");
}

void DispatchLoop() {
    unsigned int localBufferIterator = 0;
    unsigned int previous;
    semaphore = CreateSemaphore(nullptr, 0, BUFFERSIZE, TEXT(SEMAPHORE_GUID));
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
            globalCallback(globalMessageBuffer[localBufferIterator]);
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
