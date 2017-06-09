rem -----------------------------------------------------------------------
rem <copyright file="docFX_run.bat" company="Keynetics Inc">
rem     Copyright Keynetics. All rights reserved.
rem </copyright>
rem -----------------------------------------------------------------------

cd /D "%~dp0"
rem  @echo Current directory is: %CD%

rem goto SDK
cd "..\SDK"
@echo Current directory is: %CD%

rem run documentation on localhost:8080
..\packages\docfx.console.2.16.8\tools\docfx.exe docfx.json --serve
