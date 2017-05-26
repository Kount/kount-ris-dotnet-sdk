param([Parameter(Mandatory=$true)][string[]] $ComputerName,
      [switch] $Clobber)

## Author: Joakim Svendsen
## Copyright (C) 2011, Joakim Svendsen
## All rights reserved.
## BSD 3-clause license
# 2016-01-13: v1.2 - added support for .NET 4.6.1
# 2016-05-29: v1.3 - code quality improvements, standardization
# 2016-10-10: v1.4 - added support for .NET 4.6.2

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$StartTime = Get-Date
function ql { $args }
"Script start time: $StartTime"
$Date = (Get-Date).ToString('yyyy-MM-dd')
$OutputOnlineFile  = ".\DotNetOnline-${Date}.txt"
$OutputOfflineFile = ".\DotNetOffline-${Date}.txt"
$CsvOutputFile = ".\DotNet-Versions-${Date}.csv"
if (-not $Clobber) {
    $FoundExistingLog = $false
    foreach ($File in $OutputOnlineFile, $OutputOfflineFile, $CsvOutputFile) {
        if (Test-Path -PathType Leaf -Path $File) {
            $FoundExistingLog = $true
            "$File already exists"
        }
    }
    if ($FoundExistingLog -eq $true) {
        $Answer = Read-Host "The above mentioned log file(s) exist. Overwrite? [yes]"
        if ($Answer -imatch '^n') {
            Write-Error -Message 'User aborted due to not wanting to overwrite existing files' -ErrorAction Stop
            exit 1 # should be redundant
        }
    }
}
# Deleting existing log files if they exist (assume they can be deleted...)
Remove-Item $OutputOnlineFile -ErrorAction SilentlyContinue
Remove-Item $OutputOfflineFile -ErrorAction SilentlyContinue
Remove-Item $CsvOutputFile -ErrorAction SilentlyContinue
$Counter    = 0
$DotNetData = @{}
$DotNetVersionStrings = ql v4\Client v4\Full v3.5 v3.0 v2.0.50727 v1.1.4322
$DotNetRegistryBase   = 'SOFTWARE\Microsoft\NET Framework Setup\NDP'
foreach ($Computer in $ComputerName) {
    $Counter++
    $DotNetData.$Computer = New-Object PSObject
    # Skip malformed lines (well, some of them)
    if ($Computer -notmatch '^\S') {
        Write-Host -Fore Red "Skipping malformed item/line ${Counter}: '$Computer'"
        Add-Member -Name Error -Value "Malformed argument ${Counter}: '$Computer'" -MemberType NoteProperty -InputObject $DotNetData.$Computer
        continue
    }
    if (Test-Connection -Quiet -Count 1 $Computer) {
        Write-Host -Fore Green "$Computer is online. Trying to read registry."
        $Computer | Add-Content $OutputOnlineFile
        # Suppress errors when trying to open the remote key
        $ErrorActionPreference = 'SilentlyContinue'
        $Registry = [Microsoft.Win32.RegistryKey]::OpenRemoteBaseKey('LocalMachine', $Computer)
        $RegSuccess = $?
        $ErrorActionPreference = 'Stop'
        if ($RegSuccess) {
            Write-Host -Fore Green "Successfully connected to registry of ${Computer}. Trying to open keys."
            foreach ($VerString in $DotNetVersionStrings) {
                if ($RegKey = $Registry.OpenSubKey("$DotNetRegistryBase\$VerString")) {
                    #"Successfully opened .NET registry key (SOFTWARE\Microsoft\NET Framework Setup\NDP\$verString)."
                    if ($RegKey.GetValue('Install') -eq '1') {
                        #"$computer has .NET $verString"
                        Add-Member -Name $VerString -Value 'Installed' -MemberType NoteProperty -InputObject $DotNetData.$Computer
                    }
                    else {
                        Add-Member -Name $VerString -Value 'Not installed' -MemberType NoteProperty -InputObject $DotNetData.$Computer
                    }
                }
                else {
                    Add-Member -Name $VerString -Value 'Not installed (no key)' -MemberType NoteProperty -InputObject $DotNetData.$Computer
                }
            }
            # Tacking on 4.5.x and 4.6 detection, as someone requested... this script really needs a rewrite to be
            # more standards-conforming, but I'm mentally exhausted.
            # 2016-01-13: Adding 4.6.1.
            # 2016-10-10: Added 4.6.2. (rewrote parts earlier). I guess this is moving into the land of
            # where a switch statement is better suited ...
            $RegKey = $Null
            if ($RegKey = $Registry.OpenSubKey("SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full")) {
                if ($DotNet4xRelease = [int] $RegKey.GetValue('Release')) {
                    if ($DotNet4xRelease -ge 394806) {
                        $DotNetData.$Computer | Add-Member -MemberType NoteProperty -Name '>=4.x' -Value '4.6.2 or later'
                    }
                    elseif ($DotNet4xRelease -ge 394254) {
                        $DotNetData.$Computer | Add-Member -MemberType NoteProperty -Name '>=4.x' -Value '4.6.1 or later'
                    }
                    elseif ($DotNet4xRelease -ge 393295) {
                        $DotNetData.$Computer | Add-Member -MemberType NoteProperty -Name '>=4.x' -Value '4.6 or later'
                    }
                    elseif ($DotNet4xRelease -ge 379893) {
                        $DotNetData.$Computer | Add-Member -MemberType NoteProperty -Name '>=4.x' -Value '4.5.2 or later'
                    }
                    elseif ($DotNet4xRelease -ge 378675) {
                        $DotNetData.$Computer | Add-Member -MemberType NoteProperty -Name '>=4.x' -Value '4.5.1 or later'
                    }
                    elseif ($DotNet4xRelease -ge 378389) {
                        $DotNetData.$Computer | Add-Member -MemberType NoteProperty -Name '>=4.x' -Value '4.5 or later'
                    }
                    else {
                        $DotNetData.$Computer | Add-Member -MemberType NoteProperty -Name '>=4.x' -Value 'Universe imploded'
                    }
                }
                else {
                    $DotNetData.$Computer | Add-Member -MemberType NoteProperty -Name '>=4.x' -Value "Error (no 'Release' key?)"
                }
            }
            else {
                $DotNetData.$Computer | Add-Member -MemberType NoteProperty -Name '>=4.x' -Value 'Not installed (no key)'
            }
        }
        # Error opening remote registry
        else {
            Write-Host -Fore Yellow "${Computer}: Unable to open remote registry key."
            Add-Member -Name Error -Value "Unable to open remote registry: $($Error[0].ToString())" -MemberType NoteProperty -InputObject $DotNetData.$Computer
            $DotNetData.$Computer | Add-Member -MemberType NoteProperty -Name '>=4.x' -Value 'Unknown'
        }
    }
    # Failed ping test
    else {
        Write-Host -Fore Yellow "${Computer} is offline."
        Add-Member -Name Error -Value "No ping reply" -MemberType NoteProperty -InputObject $DotNetData.$Computer
        $Computer | Add-Content $OutputOfflineFile
    }    
}
$CsvHeaders = @('>=4.x') + @($DotNetVersionStrings) + @('Error')
$DotNetData.GetEnumerator() | Sort -Property Name | foreach {
    $c = $_.Name
    $_.Value | Select -Property $CsvHeaders
} | Select @{n='ComputerName';e={$c}}, * | Export-Csv -Encoding UTF8 -LiteralPath $CsvOutputFile
@"
Script start time: $StartTime
Script end time:   $(Get-Date)
Output files: $CsvOutputFile, $OutputOnlineFile, $OutputOfflineFile
"@