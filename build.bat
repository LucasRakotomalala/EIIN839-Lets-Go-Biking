@ECHO OFF
TITLE Let's Go Biking ! Build Executables

SETLOCAL EnableDelayedExpansion

SET VSWHERE="%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
FOR /f "tokens=1 delims=;" %%i in ('"!VSWHERE!" -nologo -latest -property installationPath') do SET "MSBUILDROOT=%%i"
FOR /f "tokens=1" %%i in ('"!VSWHERE!" -property installationVersion') do SET "MSBUILDVER=%%i"
SET MSBUILDPATH="!MSBUILDROOT!\MSBuild\!MSBUILDVER:~0,2!.0\Bin\MSBuild.exe"
IF NOT EXIST "!MSBUILDPATH!" SET MSBUILDPATH="!MSBUILDROOT!\MSBuild\Current\Bin\amd64\MSBuild.exe"
IF NOT EXIST "!MSBUILDPATH!" SET MSBUILDPATH="!MSBUILDROOT!\MSBuild\Current\Bin\MSBuild.exe"

ECHO Compilation de la solution ...
@ECHO.
!MSBUILDPATH! -restore /p:Configuration=Release /verbosity:quiet

ECHO La solution a bien ‚t‚ compil‚e (s'il n'y a pas d'erreur au dessus)
@ECHO.

IF NOT EXIST build mkdir build

xcopy Host\Bin\Release build /s /y /q
xcopy HeavyClient\Bin\Release build /s /y /q

@ECHO.
ECHO Les ex‚cutables sont disponibles dans le dossier 'build' (s'il n'y avait pas d'erreurs lors de la compilation)