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
void dispatchForever();
DLL void SetHook(WH_MessageCallBack progressCallback);
DLL void RemoveHook();
LRESULT CALLBACK procMessage(int nCode, WPARAM wParam, LPARAM lParam);
HANDLE mtx = NULL; //mutex
std::thread dispatcherthread;

HINSTANCE hInstHookDll = NULL; //stores instance of DLL
#endif