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


#define DLL extern "C" __declspec(dllexport)
#define BUFFERSIZE 2048
#define SEMAPHORE_GUID "7c4db072-3baf-457f-8259-da0c369e3ec8"

typedef struct {
    UINT Type;
    HWND Hwnd;
    WPARAM wParam;
    POINT Coordinates;
} WM_Message; //message struct used to send to the MORR application
typedef void(__stdcall* WH_MessageCallBack)(WM_Message);
#pragma data_seg("Shared")

HWINEVENTHOOK g_hook;
//our hook handle which will be returned by calling SetWindowsHookEx function
HHOOK GetMessageHook = NULL;
HHOOK CallWndProcHook = NULL;
ULONG bufferIterator = 0;

WH_MessageCallBack globalCallback;

DWORD timeStamps[BUFFERSIZE] = { 0 };
WM_Message lastMsg[BUFFERSIZE] = { 0 };
#pragma data_seg() //end of our data segment

#pragma comment(linker,"/section:Shared,rws")

void dispatchForever();
DLL bool IsCaptured(UINT type);
DLL void SetHook(WH_MessageCallBack wh_messageCallBack);
DLL void RemoveHook();
LRESULT CALLBACK GetMsgProc(int nCode, WPARAM wParam, LPARAM lParam);
LRESULT CALLBACK CallWndProc(int nCode, WPARAM wParam, LPARAM lParam);
HANDLE semaphore = NULL;
bool running = 0;
std::thread dispatcherthread;

HINSTANCE hInstHookDll = NULL; //stores instance of DLL
#endif