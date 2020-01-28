#ifndef DLLMAIN_H
#define DLLMAIN_H
#include "pch.h"
#include <Windows.h>
#include <thread>
#define MUTEXNAME "MORR_MSG_MUTEX"
#define DLL extern "C" __declspec(dllexport)
typedef void(__stdcall* WH_MessageCallBack)(HWND, UINT, POINT);
typedef struct {
    HWND hwnd;
    UINT message;
    POINT point;
} StoredMSG; //if i save the message itself like MSG storedMSG = &msg, the handles won't be preserved
#pragma data_seg("Shared")

HWINEVENTHOOK g_hook;
//our hook handle which will be returned by calling SetWindowsHookEx function
HHOOK hkKey = NULL;
int counter = 0;
bool newMsg = 0;

WH_MessageCallBack globalCallback;

StoredMSG lastMsg = { 0, 0, {0, 0} };
#pragma data_seg() //end of our data segment

#pragma comment(linker,"/section:Shared,rws")

void dispatchForever();
DLL void SetHook(WH_MessageCallBack progressCallback);
DLL void RemoveHook();
LRESULT CALLBACK procMessage(int nCode, WPARAM wParam, LPARAM lParam);
HANDLE mtx = NULL; //mutex
std::thread dispatcherthread;

HINSTANCE hInstHookDll = NULL; //stores instance of DLL
#endif