/**
    This DLL allows hooking into processes' windows and retreiving
    information on messages sent to them.
*/

#ifndef DLLMAIN_H
#define DLLMAIN_H
#define WIN32_LEAN_AND_MEAN

#include <Windows.h>
#include <thread>
#include <synchapi.h>

/**
    A serializable structure used for sending message data to the MORR application.
 */
struct WM_Message {
    HWND Hwnd;
    WPARAM wParam;
    UINT32 Type;
    INT32 data[4];
    void Set(UINT32 type, HWND hwnd, WPARAM wParam);
};

/**
    Helper definitions for exported and retrieved functions.
 */
#define DLL extern "C" __declspec(dllexport)
typedef void(__stdcall* WH_MessageCallBack)(WM_Message);

/**
    Size of the message buffer.
 */
#define BUFFERSIZE 2048
#define MESSAGEBUFFEROFFSET 64
#define MESSAGETABLESIZE 1025
#define TIMESTAMPSBUFFEROFFSET (MESSAGEBUFFEROFFSET + BUFFERSIZE * sizeof(WM_Message))
#define HASLISTENEROFFSET (TIMESTAMPSBUFFEROFFSET + BUFFERSIZE * sizeof(DWORD))
#define SHAREDBUFFERSIZE (HASLISTENEROFFSET + MESSAGETABLESIZE * sizeof(bool))
 /**
     Identifier of the inter-process semaphore.
  */
#define SEMAPHORE_GUID "7c4db072-3baf-457f-8259-da0c369e3ec8"
#define SHARED_MEMORY_GUID "b8befd3b-318a-4ab7-9601-0d098cafae0b"

  /**
      The iterator determining where in the buffer to store the next message.
  */
UINT* globalBufferIterator = nullptr;

/**
    Ringbuffers for the stored messages and their timestamps.
    The latter is a helper array which won't get productively.
    When using synchronization techniques in the message-loop,
    events sometimes get duplicated. These timestamps are used
    to identify if the currently inspected event is a duplicate
    of the last one by matching their exact timestamps.
 */
DWORD* globalTimeStamps = nullptr;
WM_Message* globalMessageBuffer = nullptr;

/**
    An individual table entry has to be set to true if any listener wants this message information.
    Used to prevent unnecessary processing of undesired messages.
 */
bool* messageHasListener;

/**
    Boolean value stating if the hooks are currently attached.
 */
bool* running;

#pragma data_seg("Shared")
/**
    Shared data segment accessible by all injected applications and MORR itself.
 */

/**
    Hook handles.
 */
HHOOK GetMessageHook = nullptr;
HHOOK CallWndProcHook = nullptr;


#pragma data_seg() /* end of shared data segment */

#pragma comment(linker,"/section:Shared,rws")

/**
    The callback function to be invoked inside the MORR address space to send WM_Message structs
    to the C# application.
 */
WH_MessageCallBack globalCallback;

/**
    Function running in a separate thread in the context of MORR to send event data
    from the C++ part to C#.
 */
void DispatchLoop();

/**
    Check if an event type is captured by this DLL.
    @param type The message type to check
    @returns true if the message type can be captured by this DLL.
 */
bool IsCaptured(UINT type);


/**
    Signal that message of a specific type shall be captured.
    Returns true if this message can be captured by the hook,
    false if type is unsupported.
    @param type the message type to capture
 */
DLL bool Capture(UINT type);

/**
    Signal that message of a specific type shall not be captured.
    @param type the message type to capture
 */
DLL void StopCapture(UINT type);

/**
    Set the Hook(s) and capture messages.
    @param wh_messageCallback the function to invoke and captured message strcuts.
    @param blocking determines if the function should block as long as the hook is active
 */
DLL void SetHook(WH_MessageCallBack wh_messageCallBack, bool blocking);

/**
    Remove all active hooks.
 */
DLL void RemoveHook();

/**
    Callback to be invoked on messages captured my WH_GETMESSAGE hook
 */
LRESULT CALLBACK GetMsgProc(int nCode, WPARAM wParam, LPARAM lParam);

/**
    Callback to be invoked on messages captured my WH_CALLWNDPROC hook
 */
LRESULT CALLBACK CallWndProc(int nCode, WPARAM wParam, LPARAM lParam);

bool MapSharedMemory();

void UnmapSharedMemory();

/**
    The inter-process semaphore. Each process may create this HANDLE itself if necessary.
 */
HANDLE semaphore = nullptr;

/**
    Thread running in the context of MORR, responsible for taking freshly stored messaged from the ringbuffer
    and invoking a callback on MORR to make them available to the C# code.
 */
std::thread* dispatcherthread;

/**
    The instance of this DLL. Needed for setting the hooks.
 */
HINSTANCE hInstHookDll = nullptr;

HANDLE hMapFile = nullptr;
BYTE* sharedBuffer = nullptr;

#ifdef _WIN64
/*STARTUPINFO si;
PROCESS_INFORMATION pi;
bool startWin32Helper();
void joinWin32Helper();
*/
#endif

#endif