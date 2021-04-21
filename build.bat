TITLE Let's Go Biking ! Build Executable

"C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" -restore
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" -restore

"D:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" -restore
"D:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" -restore

REM Use valide msbuild ?????

if not exist build mkdir build

xcopy Host\Bin\Debug build /s /y

xcopy HeavyClient\Bin\Debug build /s /y