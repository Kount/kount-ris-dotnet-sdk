rem -----------------------------------------------------------------------
rem <copyright file="build_sdk.bat" company="Keynetics Inc">
rem     Copyright Keynetics. All rights reserved.
rem </copyright>
rem -----------------------------------------------------------------------

rem ------------------------------------------------------------------------------
rem    PowerShell have to be installed
rem ------------------------------------------------------------------------------


rem change current dirrectory
cd /D "%~dp0"
@echo Current directory is: %CD%

powershell.exe -File buildSdk.ps1
