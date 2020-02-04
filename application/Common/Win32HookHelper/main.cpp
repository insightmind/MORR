/**
    This programs whole purpose is serving as a host
    for the 32bit HookLibrary. It will attempt to set
    the 32bit hooks and block until MORR sets the
    status of the GlobalHook to inactive (GlobalHook.IsActive = false).
    When this happens, this program will (hopefully) terminate.
 */

#include <stdio.h>
#include "Win32HookHelper.h"

int main() {
    printf("Win32HookHelper started\n");
    if (!Win32HookHelper::init()) {
        fprintf(stderr, "Closing Win32HookHelper (unknown error encountered)\n");
    }
    Win32HookHelper::freeResources();
    return 0;
}