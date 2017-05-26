<#
.SYNOPSIS 
  Test web server SSL/TLS protocol support, which could help us to configure SSL/TLS protocols and cipher suites for internal web servers.
  Clone of tracert.exe, which is a clone of the unix utility traceroute
.DESCRIPTION 
 Runs SSL diagnostic(SSLv3, TLS 1.0, TLS 1.1 and TLS 1.2) and a traceroute and returns the result.
.INPUTS 
Parameter: -RemoteHost  
    Mandatory string (example: risk.beta.kount.net)
#>
param([Parameter(Mandatory=$true)]
	  [string] $RemoteHost)


function Test-TCPPortConnection {
<#
 .SYNOPSIS
 Test the response of a computer to a specific TCP port

 .DESCRIPTION
 Test the response of a computer to a specific TCP port

 .PARAMETER  ComputerName
 Name of the computer to test the response for


 .PARAMETER  Port
 TCP Port number(s) to test

.INPUTS
 System.String.
 System.Int.

.OUTPUTS
 None

 .EXAMPLE
 PS C:\> Test-TCPPortConnection -ComputerName Server01

 .EXAMPLE
 PS C:\> Get-Content Servers.txt | Test-TCPPortConnection -Port 22,443
#>

[CmdletBinding()][OutputType('System.Management.Automation.PSObject')]

param(

[Parameter(Position=0,Mandatory=$true,HelpMessage="Name of the computer to test",
 ValueFromPipeline=$True,ValueFromPipelineByPropertyName=$true)]
 [Alias('CN','__SERVER','IPAddress','Server')]
 [String[]]$ComputerName,

 [Parameter(Position=1)]
 [ValidateRange(1,65535)]
 [Int[]]$Port = 3389
 )

	begin {
	 $TCPObject = @()
	 }

	 process {
		 foreach ($Computer in $ComputerName){
			 foreach ($TCPPort in $Port){
				  $Connection = New-Object Net.Sockets.TcpClient
				  try {
					   $Connection.Connect($Computer,$TCPPort)
					   if ($Connection.Connected) {
						   $Response = “Open”
						   $Connection.Close()
						}

				  }
				  catch [System.Management.Automation.MethodInvocationException] {
					$Response = “Closed / Filtered”
				  }
				  $Connection = $null
				  $hash = @{
					   ComputerName = $Computer
					   Port = $TCPPort
					   Response = $Response
				  }
				  $Object = New-Object PSObject -Property $hash
				  $TCPObject += $Object
			 }
		 }
	 }
	 end {
		Write-Output $TCPObject
	 }
} # end function



function Invoke-Traceroute{
<#
.SYNOPSIS 
Clone of tracert.exe, which is a clone of the unix utility traceroute
.DESCRIPTION 
Runs a traceroute and returns the result.
.INPUTS 
Pipeline 
    You can pipe -Destination from the pipeline
#>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$true,Position=1)]
        [ValidateNotNullOrEmpty()]
        [string]$Destination,

        [Parameter(Mandatory=$false)]
        [int]$MaxTTL=16,

        [Parameter(Mandatory=$false)]
        [bool]$Fragmentation=$false,

        [Parameter(Mandatory=$false)]
        [bool]$VerboseOutput=$true,

        [Parameter(Mandatory=$false)]
        [int]$Timeout=5000
    )

    $ping = new-object System.Net.NetworkInformation.Ping
    $success = [System.Net.NetworkInformation.IPStatus]::Success
    $results = @()

    if($VerboseOutput){Write-Host "Tracing to $Destination"}
    for ($i=1; $i -le $MaxTTL; $i++) {
        $popt = new-object System.Net.NetworkInformation.PingOptions($i, $Fragmentation)   
        $reply = $ping.Send($Destination, $Timeout, [System.Text.Encoding]::Default.GetBytes("MESSAGE"), $popt)
        $addr = $reply.Address

        try{$dns = [System.Net.Dns]::GetHostEntry($addr)}
        catch{$dns = "-"}

		#if ($dns -eq "-")
		#{
		#	$testPing = Test-NetConnection -ComputerName $addr -RemotePort 443
		#}

        $name = $dns.HostName
		$aList = $dns.AddressList

        $obj = New-Object -TypeName PSObject
        $obj | Add-Member -MemberType NoteProperty -Name hop -Value $i
        $obj | Add-Member -MemberType NoteProperty -Name address -Value $addr
        $obj | Add-Member -MemberType NoteProperty -Name dns_name -Value $name
        $obj | Add-Member -MemberType NoteProperty -Name latency -Value $reply.RoundTripTime

        if($VerboseOutput){Write-Host "Hop: $i`t= $addr`t($name)"}
        $results += $obj

        if($reply.Status -eq $success){break}
    }

    Return $results
}

<#
.SYNOPSIS 
Test web server SSL/TLS protocol support 
.DESCRIPTION 
Function that accepts a list of web URLs and tests each host with a list of SSL protocols: SSLv3, TLS 1.0, TLS 1.1 and TLS 1.2.
.INPUTS 
Pipeline 
    Example: "www.inbox.lv","www.paypal.com","ib.dnb.lv" | Test-ServerSSLSupport
	Default port: 443
#>
function Test-ServerSSLSupport {
[CmdletBinding()]
    param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [ValidateNotNullOrEmpty()]
        [string]$HostName,
        [Parameter(Mandatory = $false)]
        [UInt16]$Port = 443
    )
    process {
        $RetValue = New-Object psobject -Property @{
            Host = $HostName
            Port = $Port
            SSLv3 = $false
            TLSv1_0 = $false
            TLSv1_1 = $false
            TLSv1_2 = $false
            KeyExhange = $null
            HashAlgorithm = $null
        }
        "ssl3", "tls", "tls11", "tls12" | %{
            $TcpClient = New-Object Net.Sockets.TcpClient
            $TcpClient.Connect($RetValue.Host, $RetValue.Port)
            $SslStream = New-Object Net.Security.SslStream $TcpClient.GetStream()
            $SslStream.ReadTimeout = 15000
            $SslStream.WriteTimeout = 15000
            try {
                $SslStream.AuthenticateAsClient($RetValue.Host,$null,$_,$false)
                $RetValue.KeyExhange = $SslStream.KeyExchangeAlgorithm
                $RetValue.HashAlgorithm = $SslStream.HashAlgorithm
                $status = $true
            } catch {
                $status = $false
            }
            switch ($_) {
                "ssl3" {$RetValue.SSLv3 = $status}
                "tls" {$RetValue.TLSv1_0 = $status}
                "tls11" {$RetValue.TLSv1_1 = $status}
                "tls12" {$RetValue.TLSv1_2 = $status}
            }
			# get additional information
            Write-Output "----------------------------------- $_ ---------------------------------------"
			"From "+ $TcpClient.client.LocalEndPoint.address.IPAddressToString +" to $HostName - "+ $TcpClient.client.RemoteEndPoint.address.IPAddressToString +':'+$TcpClient.client.RemoteEndPoint.port
			$SslStream |gm |?{$_.MemberType -match 'Property'}|Select-Object Name |%{$_.Name +': '+ $sslStream.($_.name)}
            # dispose objects to prevent memory leaks
            $TcpClient.Dispose()
            $SslStream.Dispose()
        }

        $RetValue
    }
}


Test-NetConnection $RemoteHost -Port 443
$TestPortConnection = Test-TCPPortConnection -ComputerName $RemoteHost -Port 443

# $TestRoute = Invoke-Traceroute -Destination $RemoteHost 

$TestTLS = Test-ServerSSLSupport -HostName $RemoteHost
Write-Output "-------------------Test the response of a host: $RemoteHost to a specific TCP port--------------------------------"
Write-Output $TestPortConnection 

Write-Output $TestTLS

# Gets the list of cipher suites for TLS for a computer
# $TlsCipherSuite = Get-TlsCipherSuite -Name "TLS" 
# Write-Output "--------------Tls Cipher Suite------------------------------------"
# Write-Output $TlsCipherSuite
