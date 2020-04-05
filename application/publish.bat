@echo off
set outdir=publish
set "ModuleExt=MORR-Module.dll"
dotnet clean
dotnet publish -c release --no-dependencies -o %outdir% MORR.sln
dotnet clean
dotnet publish -c release --no-dependencies -o %outdir% -r win-x64 --self-contained true MORR.sln
md %outdir%\Modules
move %outdir%\*%ModuleExt% %outdir%\Modules\
copy build\Release\x64\HookLibrary*.dll %outdir%\
copy build\Release\x64\Win32HookHelper.exe %outdir%\
copy BuildEnvironment\config.morr %outdir%\
set /p asd="Hit enter to continue"