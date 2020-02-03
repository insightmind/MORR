#include <stdio.h>
#include "Win32HookHelper.h"

int main() {
    printf("Win32HookHelper started\n");
    Win32HookHelper::init();
    Win32HookHelper::freeResources();
    return 0;
}