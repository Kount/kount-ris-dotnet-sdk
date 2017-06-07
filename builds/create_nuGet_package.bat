rem -----------------------------------------------------------------------
rem </copyright>
rem -----------------------------------------------------------------------

cd /D "%~dp0"
@echo Current directory is: %CD%

rem run nuget.exe
nuget pack ..\SDK\KountRisSdk.csproj
