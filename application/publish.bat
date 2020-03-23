@echo off
set targetframework=netcoreapp3.1
set outdir=publishing
set moduleoutdir=publishing\Modules
set noselfcontained=( ^
Core\CLI\CLI.csproj ^
Common\Shared\Shared.csproj)
set coreprojects=( ^
Core\MORR\MORR.csproj ^
Core\UI\UI.csproj)
set noselfcontainedmodules=( ^
Modules\Mouse\Mouse.csproj ^
Modules\WindowManagement\WindowManagement.csproj)
set modules=( ^
Modules\Clipboard\Clipboard.csproj ^
Modules\Keyboard\Keyboard.csproj ^
Modules\Webbrowser\WebBrowser.csproj)
for %%i in %noselfcontained% do dotnet publish -o %outdir% -f %targetframework% -c release %%i
for %%i in %coreprojects% do dotnet publish -o %outdir% -c release -r win10-x64 -f %targetframework% --self-contained true %%i
for %%i in %modules% do dotnet publish -o %moduleoutdir% -c release -r win10-x64 -f %targetframework% --self-contained true %%i
for %%i in %noselfcontainedmodules% do dotnet publish -o %moduleoutdir% -c release -f %targetframework% %%i
set /p asd="Hit enter to continue"