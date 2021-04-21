REM msbuild -restore

if not exist build mkdir build

xcopy /s /y Host/[Bb]in/[Dd]ebug build
xcopy /s /y HeavyClient/[Bb]in/[Dd]ebug build