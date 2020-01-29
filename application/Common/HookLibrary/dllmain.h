/**
    This DLL allows hooking into processes' windows and retreiving
    information on messages sent to them.
*/

#ifndef DLLMAIN_H
#define DLLMAIN_H
#include "pch.h"
#include <Windows.h>
#include <thread>
#include <synchapi.h>

/**
    A serializable structure used for sending message data to the MORR application.
 */
typedef struct {
    UINT Type;
    HWND Hwnd;
    WPARAM wParam;
    POINT Coordinates;
} WM_Message;

/**
    Helper definitions for exported and retrieved functions.
 */
#define DLL extern "C" __declspec(dllexport)
typedef void(__stdcall* WH_MessageCallBack)(WM_Message);

/**
    Size of the message buffer.
 */
#define BUFFERSIZE 2048
 /**
     Identifier of the inter-process semaphore.
  */
#define SEMAPHORE_GUID "7c4db072-3baf-457f-8259-da0c369e3ec8"


#pragma data_seg("Shared")
/**
    Shared data segment accessible by all injected applications and MORR itself.
 */

/**
    Hook handles.
 */
HHOOK GetMessageHook = NULL;
HHOOK CallWndProcHook = NULL;

/**
    The iterator determining where in the buffer to store the next message.
*/
ULONG globalBufferIterator = 0;

/**
    The callback function to be invoked inside the MORR address space to send WM_Message structs
    to the C# application.
 */
WH_MessageCallBack globalCallback;

/**
    Ringbuffers for the stored messages and their timestamps.
    The latter is a helper array which won't get productively.
    When using synchronization techniques in the message-loop,
    events sometimes get duplicated. These timestamps are used
    to identify if the currently inspected event is a duplicate
    of the last one by matching their exact timestamps.
 */
DWORD globalTimeStamps[BUFFERSIZE] = { 0 };
WM_Message globalMessageBuffer[BUFFERSIZE] = { 0 };
#pragma data_seg() /* end of shared data segment */

#pragma comment(linker,"/section:Shared,rws")

/**
    Function running in a separate thread in the context of MORR to send event data
    from the C++ part to C#.
 */
void dispatchForever();

/**
    Check if an event type is captured by this DLL.
    @param type The message type to check
    @returns true if the message type can be captured by this DLL.
 */
DLL bool IsCaptured(UINT type);

/**
    Set the Hook(s) and capture messages.
    @param wh_messageCallback the function to invoke and captured message strcuts.
 */
DLL void SetHook(WH_MessageCallBack wh_messageCallBack);

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
    The inter-process semaphore. Each process may create this HANDLE itself if necessary.
 */
HANDLE semaphore = NULL;

/**
    Boolean value stating if the hooks are currently attached.
 */
bool running = 0;

/**
    Thread running in the context of MORR, responsible for taking freshly stored messaged from the ringbuffer
    and invoking a callback on MORR to make them available to the C# code.
 */
std::thread* dispatcherthread;

/**
    The instance of this DLL. Needed for setting the hooks.
 */
HINSTANCE hInstHookDll = NULL;
#endif