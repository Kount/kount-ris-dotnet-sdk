rem -----------------------------------------------------------------------
rem <copyright file="create_nuGet_package.bat" company="Kount Inc">
rem     Copyright Kount Inc. All rights reserved.
rem </copyright>
rem -----------------------------------------------------------------------

cd /D "%~dp0"
@echo Current directory is: %CD%

rem run nuget.exe
nuget pack ..\SDK\KountRisSdk.csproj
