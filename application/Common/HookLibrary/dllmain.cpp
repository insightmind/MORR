// dllmain.cpp : Defines the entry point for the DLL application.

#include "dllmain.h"

BOOL APIENTRY DllMain(HANDLE hModule, DWORD  ul_reason_for_call,
    LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        hInstHookDll = (HINSTANCE)hModule;
        MapSharedMemory();
        break;
    case DLL_PROCESS_DETACH:
        UnmapSharedMemory();
        break;
    }
    return TRUE;
}

LRESULT CALLBACK GetMsgProc(int nCode, WPARAM wParam, LPARAM lParam)
//this is the hook procedure
{
    LRESULT retVal = CallNextHookEx(GetMessageHook, nCode, wParam, lParam);
    if (!mappedFileHandle && !MapSharedMemory())
        goto forwardEvent;
    //a pointer to hold the MSG structure that is passed as lParam
    MSG* msg;
    if (semaphore == nullptr) {
        semaphore = CreateSemaphore(nullptr, 0, BUFFERSIZE, TEXT(SEMAPHORE_GUID));
        if (semaphore == nullptr)
            goto forwardEvent;
    }

    //lParam contains pointer to MSG structure.
    msg = (MSG*)lParam;
    /* see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644981(v=vs.85) */
    if (nCode >= 0 && nCode == HC_ACTION && msg->message < MESSAGETABLESIZE && shared_messageHasListener[msg->message])
    {
        unsigned int bufferSlot = InterlockedIncrement(shared_globalBufferIterator) % BUFFERSIZE; //atomic increment. overflows should not matter as we modulo it anyways.
        unsigned int type = msg->message;
        shared_globalTimeStamps[bufferSlot] = msg->time;
        if ((type >= WM_MOUSEMOVE && type <= WM_MOUSEWHEEL) || (type >= WM_NCMOUSEMOVE && type <= WM_NCMBUTTONDBLCLK) || type == WM_KEYDOWN) {
            shared_globalMessageBuffer[bufferSlot].Set(msg->message, msg->hwnd, msg->wParam);
            shared_globalMessageBuffer[bufferSlot].data[0] = msg->pt.x;
            shared_globalMessageBuffer[bufferSlot].data[1] = msg->pt.y;
        }
        else
        {
            shared_globalMessageBuffer[bufferSlot].Type = WM_NULL;
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
    if (!mappedFileHandle && !MapSharedMemory())
        goto forwardEvent;
    CWPSTRUCT* msg;
    if (semaphore == nullptr) {
        semaphore = CreateSemaphore(nullptr, 0, BUFFERSIZE, TEXT(SEMAPHORE_GUID));
        if (semaphore == nullptr)
            goto forwardEvent;
    }

    msg = (CWPSTRUCT*)lParam;

    /* see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644981(v=vs.85) */
    if (nCode >= 0 && nCode == HC_ACTION && msg->message < MESSAGETABLESIZE && shared_messageHasListener[msg->message])
    {
        unsigned int bufferSlot = InterlockedIncrement(shared_globalBufferIterator) % BUFFERSIZE;
        shared_globalTimeStamps[bufferSlot] = 0;
        unsigned int type = msg->message;

        /* I did opt to go for if..elif instead of switch..case since some types can be checked by range */
        if (type == WM_CREATE)
        {
            shared_globalMessageBuffer[bufferSlot].Set(msg->message, msg->hwnd, 0);
            shared_globalMessageBuffer[bufferSlot].data[0] = ((CREATESTRUCT*)msg->lParam)->x;
            shared_globalMessageBuffer[bufferSlot].data[1] = ((CREATESTRUCT*)msg->lParam)->y;
        }
        else if (type == WM_MOVE || type == WM_SIZE)
        {
            shared_globalMessageBuffer[bufferSlot].Set(msg->message, msg->hwnd, msg->wParam);
            shared_globalMessageBuffer[bufferSlot].data[0] = (INT16)LOWORD(msg->lParam);
            shared_globalMessageBuffer[bufferSlot].data[1] = (INT16)HIWORD(msg->lParam);
        }
        else if (type == WM_DESTROY || type == WM_ACTIVATE || ((type >= WM_SETFOCUS) && (type <= WM_ENABLE)))
        {
            shared_globalMessageBuffer[bufferSlot].Set(msg->message, msg->hwnd, msg->wParam);
            if (type == WM_ACTIVATE)
            {
                *((HWND*)&shared_globalMessageBuffer[bufferSlot].data[0]) = (HWND)msg->lParam;
            }

        }
        else if (type == WM_SIZING)
        {
            shared_globalMessageBuffer[bufferSlot].Set(msg->message, msg->hwnd, msg->wParam);
        }
        else if ((type >= WM_CUT && type <= WM_CLEAR)) {
            shared_globalMessageBuffer[bufferSlot].Set(msg->message, msg->hwnd, msg->wParam);
        }
        else
        {
            shared_globalMessageBuffer[bufferSlot].Type = WM_NULL;
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
        shared_messageHasListener[type] = true;
        return true;
    }
    return false;
};

void StopCapture(UINT type) {
    if (type < MESSAGETABLESIZE)
        shared_messageHasListener[type] = false;
};

DLL void SetHook(WH_MessageCallBack progressCallback, bool blocking) {
    if (GetMessageHook || CallWndProcHook) {
        /* the stored values in this DLL might be persisting even
           over program restarts if not detached properly */
        fprintf(stderr, "%s GlobalHook wasn't properly detached last time, trying to reset before attaching\n", ARCH);
        UnmapSharedMemory();
        RemoveHook();
    }
    if (!MapSharedMemory()) {
        fprintf(stderr, "Could not set %s GlobalHook (shared memory inaccessible).\n", ARCH);
    }
    /**
        Only set shared variables in 64bit version to avoid racecondition.
        (The 64bit hook might already overwrite these values before the 32 bit child would
        set these values).
     */
#ifdef _WIN64
    *shared_running = true;
    *shared_globalBufferIterator = 0;
#endif
    globalCallback = progressCallback;

    if (GetMessageHook == nullptr) {
        if ((GetMessageHook = SetWindowsHookEx(WH_GETMESSAGE, GetMsgProc, hInstHookDll, 0)) == nullptr)
            fprintf(stderr, "Error attaching %s GetMessage hook. Errorcode %d\n", ARCH, GetLastError());
    }
    if (CallWndProcHook == nullptr) {
        if ((CallWndProcHook = SetWindowsHookEx(WH_CALLWNDPROC, CallWndProc, hInstHookDll, 0)) == nullptr)
            fprintf(stderr, "Error attaching %s WndProc hook. Errorcode %d\n", ARCH, GetLastError());
    }
    if (GetMessageHook == nullptr || CallWndProcHook == nullptr) {
        RemoveHook();
        return;
    }
#ifdef _WIN64
    if (!StartWin32Helper()) {
        fprintf(stderr, "Could not launch Win32HookHelper.\n");
    }
#endif
    printf("%s GlobalHook: Hooked\n", ARCH);
    if (!blocking)
    {
        dispatcherthread = new std::thread(DispatchLoop);
    }
    else {
        DispatchLoop();
        RemoveHook();
    }
}

DLL void RemoveHook()
{
    if (shared_running != nullptr)
        *shared_running = false;
    if (GetMessageHook != nullptr) {
        if (!UnhookWindowsHookEx(GetMessageHook))
            fprintf(stderr, "Error unhooking %s GetMessage hook. Errorcode %d\n", ARCH, GetLastError());
    }
    if (CallWndProcHook != nullptr) {
        if (!UnhookWindowsHookEx(CallWndProcHook))
            fprintf(stderr, "Error unhooking %s CallWndProc hook. Errorcode %d\n", ARCH, GetLastError());
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
    shared_globalBufferIterator = nullptr;
#ifdef _WIN64
    JoinWin32Helper();
#endif
    UnmapSharedMemory();
    printf("%s GlobalHook: Unhooked\n", ARCH);
}

#ifdef _WIN64
void DispatchLoop() {
    unsigned int localBufferIterator = 0;
    unsigned int previous;
    semaphore = CreateSemaphore(nullptr, 0, BUFFERSIZE, TEXT(SEMAPHORE_GUID));
    while (*shared_running) {
        {
            while (localBufferIterator == *shared_globalBufferIterator % BUFFERSIZE) {
                if (*shared_running)
                    WaitForSingleObject(semaphore, 1000); /* wake up every second so we could react to RemoveHook call.*/
                else
                    return;
            }
            previous = localBufferIterator;
            ++localBufferIterator %= BUFFERSIZE;
            if ((shared_globalTimeStamps[localBufferIterator]) && ((shared_globalTimeStamps[localBufferIterator]) == shared_globalTimeStamps[(previous)])
                 && (shared_globalMessageBuffer[localBufferIterator].Type == shared_globalMessageBuffer[previous].Type)) /* discard event if it's obviously a duplicate (works only or events which
                                                                                                              inherently come with a timestamp) */
                continue;
            globalCallback(shared_globalMessageBuffer[localBufferIterator]);
        }
    }
}
#else
void DispatchLoop() {
    while (shared_running != nullptr && *shared_running)
        Sleep(1000);
}
#endif

bool MapSharedMemory() {
    if (!mappedFileHandle) {
        mappedFileHandle = CreateFileMapping(
            INVALID_HANDLE_VALUE,    // use paging file
            NULL,                    // default security
            PAGE_READWRITE,          // read/write access
            0,                       // maximum object size (high-order DWORD)
            SHAREDBUFFERSIZE,                // maximum object size (low-order DWORD)
            TEXT(SHARED_MEMORY_GUID));                 // name of mapping object
        if (!mappedFileHandle) {
            fprintf(stderr, "Error creating shared memory segment. Errorcode %d\n", GetLastError());
            return false;
        }
        else
        {
            sharedBuffer = (BYTE*)MapViewOfFile(mappedFileHandle,
                FILE_MAP_ALL_ACCESS,
                0,
                0,
                SHAREDBUFFERSIZE);
            if (!sharedBuffer) {
                fprintf(stderr, "Error mapping shared memory segment. Errorcode %d\n", GetLastError());
                CloseHandle(mappedFileHandle);
                mappedFileHandle = NULL;
                return false;
            }
            else
            {
                shared_running = (bool*)&sharedBuffer[0];
                shared_globalBufferIterator = (UINT*)&sharedBuffer[4];
                shared_globalMessageBuffer = (WM_Message*)&sharedBuffer[MESSAGEBUFFEROFFSET];
                shared_globalTimeStamps = (DWORD*)&sharedBuffer[TIMESTAMPSBUFFEROFFSET];
                shared_messageHasListener = (bool*)&sharedBuffer[HASLISTENEROFFSET];
            }
        }
    }
    return true;
}

void UnmapSharedMemory() {
    if (sharedBuffer) {
        UnmapViewOfFile(sharedBuffer);
        sharedBuffer = nullptr;
    }
    if (mappedFileHandle) {
        CloseHandle(mappedFileHandle);
        mappedFileHandle = nullptr;
    }
    shared_globalBufferIterator = nullptr;
    shared_globalMessageBuffer = nullptr;
    shared_globalTimeStamps = nullptr;
    shared_messageHasListener = nullptr;
    shared_running = nullptr;
    mappedFileHandle = nullptr;
    sharedBuffer = nullptr;
}

#ifdef _WIN64
bool StartWin32Helper() {
    ZeroMemory(&win32HelperStartupInfo, sizeof(win32HelperStartupInfo));
    win32HelperStartupInfo.cb = sizeof(win32HelperStartupInfo);
    ZeroMemory(&win32HelperProcessInformation, sizeof(win32HelperProcessInformation));
    wchar_t pathName[] = L"Win32HookHelper.exe";

    if (!CreateProcess(NULL,   // No module name (use command line)
        pathName,        // Command line
        NULL,           // Process handle not inheritable
        NULL,           // Thread handle not inheritable
        FALSE,          // Set handle inheritance to FALSE
        0,              // No creation flags
        NULL,           // Use parent's environment block
        NULL,           // Use parent's starting directory 
        &win32HelperStartupInfo,            // Pointer to STARTUPINFO structure
        &win32HelperProcessInformation)           // Pointer to PROCESS_INFORMATION structure
        )
    {
        return false;
    }
    return true;

}

void JoinWin32Helper() {
    WaitForSingleObject(win32HelperProcessInformation.hProcess, 5000); //wait for child to close, but for a maximum of 5 seconds

    // Close process and thread handles.
    CloseHandle(win32HelperProcessInformation.hProcess);
    CloseHandle(win32HelperProcessInformation.hThread);
}

#endif

void WM_Message::Set(UINT32 type, HWND hwnd, WPARAM wParam)
{
    this->Type = type;
    this->Hwnd = (UINT64)hwnd;
    this->wParam = (UINT64)wParam;
    ZeroMemory((void*)&this->data[0], sizeof(this->data));
}
