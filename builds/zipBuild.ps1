<#
//-----------------------------------------------------------------------
// <copyright file="zipBuild.ps1" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#>


#
# VS 2015 path must be set
# .\zipBuild.ps1 -SdkVersion 0695
#
Param(  
    [Parameter(Mandatory = $true)]
	[String]$SdkVersion = '0650'
     )

# Path for VS 2017
# $VSPath = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community" 
# $MSBuildPath = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin"

# Path for VS 2015
$VSPath = "C:\Program Files (x86)\Microsoft Visual Studio 14.0" 
$MSBuildPath = "C:\Program Files (x86)\MSBuild\14.0\Bin"

# Path for VS 2013
# $VSPath = "C:\Program Files (x86)\Microsoft Visual Studio 12.0" 
# $MSBuildPath = "C:\Program Files (x86)\MSBuild\12.0\Bin"


#
# Checks if Ris.MerchantId, Ris.Url, Ris.Config.Key are setted in App.config 
#
Function Read-TestConfig {

	$scriptPath = Split-Path -parent $PSCommandPath
	$parentProjectPath = Split-Path -parent $scriptPath
	$ConfigSourceTestPath = Echo $parentProjectPath\KountRisConfigTest
	$ConfigFile = Echo $ConfigSourceTestPath\App.config
	[xml]$xml = Get-Content $ConfigFile
    #$nodeMerchantId = Select-Xml "//appSettings[add/@key='Ris.MerchantId']" $xml
    #$nodeSalt = Select-Xml "//appSettings[add/@key='Ris.Config.Key']" $xml
    #$nodeUrl = Select-Xml "//appSettings[add/@key='Ris.Url']" $xml
    #$nodeApi = Select-Xml "//appSettings[add/@key='Ris.API.Key']" $xml
	$midValue = ""
	$saltValue = ""
	$urlValue = ""
	$apiValue = ""
    
	$addNodes = $xml.selectNodes("//appSettings/add")
	foreach ($node in $addNodes) {
		if ($node.key -eq 'Ris.MerchantId')
		{
			$midValue = Echo $node.value
		}
		if ($node.key -eq 'Ris.Config.Key')
		{
			$saltValue = Echo $node.value
		}
		if ($node.key -eq 'Ris.Url')
		{
			$urlValue = Echo $node.value
		}
		if ($node.key -eq 'Ris.API.Key')
		{
			$apiValue = Echo $node.value
		}
	}

	if ([string]::IsNullOrEmpty($midValue)){
		Write-Output "Ris.MerchantId is NOT set in App.Config to run config-dependent tests.`n"
		Write-Host "Ris.MerchantId is NOT set in App.Config to run config-dependent tests. " -ForegroundColor Red
	}

	if ([string]::IsNullOrEmpty($saltValue)){
		Write-Output "Ris.Config.Key is NOT set in App.Config to run config-dependent tests.`n"
		Write-Host "Ris.Config.Key is NOT set in App.Config to run config-dependent tests. " -ForegroundColor Red
	}

	if ([string]::IsNullOrEmpty($urlValue)){
		Write-Output "Ris.Url is NOT set in App.Config to run config-dependent tests.`n" 
		Write-Host "Ris.Url is NOT set in App.Config to run config-dependent tests. " -ForegroundColor Red
	}

	if ([string]::IsNullOrEmpty($apiValue)){
		Write-Output "Ris.API.Key is NOT set in App.Config to run config-dependent tests.`n" 
		Write-Host "Ris.API.Key is NOT set in App.Config to run config-dependent tests. " -ForegroundColor Red
	}

    $return = (-not ([string]::IsNullOrEmpty($midValue))) `
		 -and (-not ([string]::IsNullOrEmpty($saltValue))) `
		 -and (-not ([string]::IsNullOrEmpty($urlValue))) `
		 -and (-not ([string]::IsNullOrEmpty($apiValue)))
    return 
}

$TestConfig = Read-TestConfig
Write-Output $TestConfig

$VsTestPath = Echo $VSPath\Common7\IDE\CommonExtensions\Microsoft\TestWindow

$VerbosePreference = "Continue"
$ErrorActionPreference = "Stop"

$scriptPath = Split-Path -parent $PSCommandPath
$parentProjectPath = Split-Path -parent $scriptPath

# $VSPath = .\vswhere -legacy -latest -property installationPath

if (!(Test-Path $MSBuildPath\msbuild.exe)) {
	Write-Host "Cannot find MSBuild.exe" -foregroundcolor Red
	Write-Host "Please, set MSBuildPath with valid path to MSBuild." -Foregroundcolor Red
	$wshell = New-Object -ComObject Wscript.Shell
    $wshell.Popup("Please set MSBuildPath with valid path to MSBuild.",0,"MSBuild Path",0)
	Exit
}

if (!(Test-Path $VsTestPath\vstest.console.exe)) {
	Write-Host "Cannot find vstest.console.exe" -foregroundcolor "red"
	Write-Host "Please, set VSPath with valid path to latest Visual Studio instalation." -foregroundcolor "red"
	$wshell = New-Object -ComObject Wscript.Shell
    $wshell.Popup("Please set VSPath with valid path to latest Visual Studio instalation.",0,"Visual Studio instalation",0)
	Exit
}

# ####### Get installed DotNet version
$Date = (Get-Date).ToString('yyyy-MM-dd')
$CsvOutputFile = ".\DotNet-Versions-${Date}.csv"
.\Get-DotNet-Version -ComputerName $env:computername -Clobber:$true
$DotNetVer = Import-Csv $CsvOutputFile | Select -First 1
Write-Output $DotNetVer
#

$SdkPathName = "Sdk-Ris-Dotnet-"+$SdkVersion
$SdkZipName = "Sdk-Ris-Dotnet-"+$SdkVersion+"-"+[DateTime]::Now.ToString("yyyyMMddHHmm")+".zip"

$SdkDir = Echo $parentProjectPath\SDK
$SdkProj = Echo $SdkDir\KountRisSdk.csproj
$OutputSdkPath = Echo $scriptPath\$SdkPathName
$LibPath = Echo $OutputSdkPath\lib
$TargetSrc = Echo $OutputSdkPath\src
$TargetDocs = Echo $TargetSrc\docs
$TargetBuilds = Echo $TargetSrc\builds
$TargetSrcSDKPath = Echo $OutputSdkPath\src\SDK

$SourceSDK = Echo $SdkDir\*
$SourceDOCS = Echo $parentProjectPath\docs\*
$SourceBuilds = Echo $parentProjectPath\builds\*
# Test project
$SourceTest = Echo $parentProjectPath\KountRisTest\*
$TestProj = Echo $parentProjectPath\KountRisTest\KountRisTest.csproj
$TestDLL = Echo $LibPath\KountRisTest.dll
$TargetSrcTestPath = Echo $OutputSdkPath\src\KountRisTest
# Configuration-dependent test project
$ConfigSourceTest = Echo $parentProjectPath\KountRisConfigTest\*
$ConfigTestProj = Echo $parentProjectPath\KountRisConfigTest\KountRisConfigTest.csproj
$ConfigTestDLL = Echo $LibPath\KountRisConfigTest.dll
$ConfigTargetSrcTestPath = Echo $OutputSdkPath\src\KountRisConfigTest

$TestResDir = Echo $scriptPath\TestResults
$SdkTestResDir = Echo $SdkPathName\TestResults

# "C:\Program Files (x86)\NuGet\nuget.exe" restore $parentProjectPath\KountSdk.sln
# Restore packages for a solution file, using MSBuild version 14.0 to load the solution and its project(s)
# nuget.exe restore $parentProjectPath\KountSdk.sln -MSBuildVersion 14 
& $scriptPath\nuget.exe restore $parentProjectPath\KountSdk.sln -Verbosity quiet

#
# "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild" ..\SDK\KountRisSdk.csproj /target:build /p:OutputPath=lib /p:Configuration=Release
#
& $MSBuildPath\msbuild.exe $SdkProj `
								/p:Configuration=Release `
								/p:OutputPath=$LibPath `
								/t:build 

& $MSBuildPath\msbuild.exe $TestProj `
								/p:Configuration=Release `
								/p:OutputPath=$LibPath `
								/t:build 

& $MSBuildPath\msbuild.exe $ConfigTestProj `
								/p:Configuration=Release `
								/p:OutputPath=$LibPath `
								/t:build 

#
# "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
#
& $VsTestPath\vstest.console.exe $TestDLL /Logger:trx 

if ([string]::IsNullOrEmpty($TestConfig))
{
	& $VsTestPath\vstest.console.exe $ConfigTestDLL /Logger:trx 
}
else{
	Write-Host "Config-dependent tests not passed successfully. Please, add required settings." -foregroundcolor "red"
	$wshell = New-Object -ComObject Wscript.Shell
    $wshell.Popup($TestConfig,0,"Config-dependent tests CHECK",0)
}

# Copy configuration-dependent test sorce project
$excludes = "bin","obj","Debug","Release"
Get-ChildItem $ConfigSourceTest -Directory | 
	Where-Object{$_.Name -notin $excludes} | 
	Copy-Item -Destination $ConfigTargetSrcTestPath -Force 
Copy-Item $ConfigSourceTest $ConfigTargetSrcTestPath -Force -Recurse

# Copy test sorce project
$excludes = "bin","obj","Debug","Release"
Get-ChildItem $SourceTest -Directory | 
    Where-Object{$_.Name -notin $excludes} | 
    Copy-Item -Destination $TargetSrcTestPath -Force 
Copy-Item $SourceTest $TargetSrcTestPath -Force -Recurse

# Copy SDK source
Get-ChildItem $SourceSDK -Directory | 
    Where-Object{$_.Name -notin $excludes} | 
    Copy-Item -Destination $TargetSrcSDKPath -Force 

Copy-Item $SourceSDK $TargetSrcSDKPath -Force -Recurse

# Copy documents
New-Item $TargetDocs -itemtype directory
Copy-Item $SourceDOCS -Destination $TargetDocs -Force -Recurse

# Copy builds
New-Item $TargetBuilds -itemtype directory
Copy-Item $scriptPath\buildSdk.ps1 -Destination $TargetBuilds -Force 
Copy-Item $scriptPath\Get-DotNet-Version.ps1 -Destination $TargetBuilds -Force 
Copy-Item $scriptPath\zipBuild.ps1 -Destination $TargetBuilds -Force 
Copy-Item $scriptPath\build_sdk.bat -Destination $TargetBuilds -Force 
Copy-Item $scriptPath\docFX_run.bat -Destination $TargetBuilds -Force 
Copy-Item $scriptPath\nuget.exe -Destination $TargetBuilds -Force 

# Copy test result files
New-Item $SdkTestResDir -itemtype directory
#  Get TRX files
$TrxFilesList = Get-ChildItem $TestResDir\*.trx
$TrxFiles = ''  
$loop = 1
foreach($file in $TrxFilesList)  
{   
    $TrxFiles = "$file" 
    # copy the trx file into a space-less named trx
    $newTrxFileName = "KountRisTest" + "_" + "$loop" + ".trx"

    copy-item $TrxFiles -destination $SdkTestResDir\$newTrxFileName

    $loop = $loop + 1
    $newTrxFile += "$SdkTestResDir\$newTrxFileName"
    $newTrxFile += " "
}


# Copy solution file
Copy-Item $parentProjectPath\KountSdk.sln -Destination $TargetSrc -Force

Add-Type -A System.IO.Compression.FileSystem
[IO.Compression.ZipFile]::CreateFromDirectory($OutputSdkPath, $SdkZipName)

Remove-Item $SdkPathName -Force -Recurse
Remove-Item $TestResDir -Force -Recurse

#Get-ChildItem -Path $SdkPathName -Include * | Remove-Item -recurse 
# Remove-Item $SdkZipName

