TITLE Let's Go Biking ! Build Executables

SETLOCAL EnableDelayedExpansion
SET VSWHERE="%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
FOR /f "tokens=1 delims=;" %%i in ('"!VSWHERE!" -nologo -latest -property installationPath') do SET "MSBUILDROOT=%%i"
FOR /f "tokens=1" %%i in ('"!VSWHERE!" -property installationVersion') do SET "MSBUILDVER=%%i"
SET MSBUILDPATH="!MSBUILDROOT!\MSBuild\!MSBUILDVER:~0,2!.0\Bin\MSBuild.exe"
IF NOT EXIST "!MSBUILDPATH!" SET MSBUILDPATH="!MSBUILDROOT!\MSBuild\Current\Bin\amd64\MSBuild.exe"
IF NOT EXIST "!MSBUILDPATH!" SET MSBUILDPATH="!MSBUILDROOT!\MSBuild\Current\Bin\MSBuild.exe"

!MSBUILDPATH! -restore /p:Configuration=Release

IF NOT EXIST build mkdir build

xcopy Host\Bin\Release build /s /y
xcopy HeavyClient\Bin\Release build /s /y