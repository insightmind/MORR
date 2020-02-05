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
    Do not use pointer-types for HWND and WPARAM as this would complicate 32bit interoperability.
 */
struct WM_Message {
    volatile UINT64 Hwnd;
    volatile UINT64 wParam;
    volatile UINT32 Type;
    volatile INT32 data[5];
    void Set(UINT32 type, HWND hwnd, WPARAM wParam);
};

/**
    Helper definitions for exported and retrieved functions.
 */
#define DLL extern "C" __declspec(dllexport)
typedef void(__stdcall* WH_MessageCallBack)(WM_Message);

/**
    Amount of array entries in the message ringbuffer.
 */
#define BUFFERSIZE 2048

 /**
     Size of the message table, which indicates which messages shall be captured/forwarded to MORR.
     As currently no relevant message has an integer value of over 1025, that is the current choice.
  */
#define MESSAGETABLESIZE 1025

/**
    Starting indices of the message buffer, timestamp buffer
    and hasListener array in the shared buffer. First 64 bytes
    are used for global primitives.
 */
#define MESSAGEBUFFEROFFSET 64
#define TIMESTAMPSBUFFEROFFSET (MESSAGEBUFFEROFFSET + BUFFERSIZE * sizeof(WM_Message))
#define HASLISTENEROFFSET (TIMESTAMPSBUFFEROFFSET + BUFFERSIZE * sizeof(DWORD))

/**
    The required size of the shared memory region in bytes.
 */
#define SHAREDBUFFERSIZE (HASLISTENEROFFSET + MESSAGETABLESIZE * sizeof(bool))

 /**
     Identifier of the inter-process named semaphore.
  */
#define SEMAPHORE_GUID "7c4db072-3baf-457f-8259-da0c369e3ec8"
  /**
      Identifier of the named shared memory region.
   */
#define SHARED_MEMORY_GUID "b8befd3b-318a-4ab7-9601-0d098cafae0b"

/**
    Define string contants used in debug output.
 */
#ifdef _WIN64
#define ARCH "64bit"
#else
#define ARCH "32bit"
#endif

  /**
      The iterator determining where in the buffer to store the next message.
  */
volatile UINT* shared_globalBufferIterator = nullptr;

/**
    Ringbuffers for the stored messages and their timestamps.
    The latter is a helper array which won't get productively.
    When using synchronization techniques in the message-loop,
    events sometimes get duplicated. These timestamps are used
    to identify if the currently inspected event is a duplicate
    of the last one by matching their exact timestamps.
 */
volatile DWORD* shared_globalTimeStamps = nullptr;
WM_Message* shared_globalMessageBuffer = nullptr;

/**
    An individual table entry has to be set to true if any listener wants this message information.
    Used to prevent unnecessary processing of undesired messages.
 */
volatile bool* shared_messageHasListener = nullptr;

/**
    Boolean value stating if the hooks are currently attached.
 */
volatile bool* shared_running = nullptr;

/**
    Shared data segment accessible by all injected applications and MORR itself.
 */
#pragma data_seg("Shared")

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

/**
    Map the shared memory region and define the respective pointers.
 */
bool MapSharedMemory();

/**
    Unmap the shared memory region and null all respective pointers.
 */
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

/**
    Handles for the shared memory region.
    Sharedbuffer will point to the first address of the shared memory region.
 */
HANDLE mappedFileHandle = nullptr;
BYTE* sharedBuffer = nullptr;

#ifdef _WIN64
/**
    Variables and functions used to manage the Win32HookHelper.
    As the Win32HookHelper is a child of the 64bit process,
    those must only exist in the 64bit version.
 */
STARTUPINFO win32HelperStartupInfo;
PROCESS_INFORMATION win32HelperProcessInformation;

/**
    Start the 32bit child process.
 */
bool StartWin32Helper();

/**
    Join the 32bit subprocess and close the handles.
    Will only succeed if *running  is not true
 */
void JoinWin32Helper();
#endif

#endif