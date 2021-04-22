TITLE Let's Go Biking ! Build Executable

REM "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" -restore
REM "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" -restore

REM  "D:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" -restore
REM "D:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" -restore

msbuild "Let's Go Biking.sln" /p:Configuration=Release

if not exist build mkdir build

xcopy Host\Bin\Release build /s /y

xcopy HeavyClient\Bin\Release build /s /y