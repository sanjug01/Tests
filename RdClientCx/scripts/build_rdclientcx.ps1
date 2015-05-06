$SDXROOT = "e:\sdx\clouddv"

Function BcxPlatform($platform)
{
	$pinfo = New-Object System.Diagnostics.ProcessStartInfo
	$pinfo.FileName = "cmd.exe"
	$pinfo.RedirectStandardInput = $true
	$pinfo.RedirectStandardError = $false
	$pinfo.RedirectStandardOutput = $false
	$pinfo.UseShellExecute = $false
	$p = New-Object System.Diagnostics.Process
	$p.StartInfo = $pinfo
	$p.Start()
	$p.StandardInput.WriteLine("cd /D $SDXROOT")
	$p.StandardInput.WriteLine(".\tools\razzle.cmd $platform no_certcheck no_oacr no_opt")
	$p.StandardInput.WriteLine("cd .\termsrv\rdp\winrt\RdClientCx\")
	$p.StandardInput.WriteLine("bcx.cmd")
	$p.StandardInput.WriteLine("exit")
	$p.WaitForExit()
}

BcxPlatform "x86chk"
BcxPlatform "x86fre"
BcxPlatform "armchk"
BcxPlatform "armfre"

xcopy $SDXROOT\testsrc\termsrvtestdata\REDIST\WinRTClientBin\Debug ..\WinRTClientBin\Debug /i /s /y
xcopy $SDXROOT\testsrc\termsrvtestdata\REDIST\WinRTClientBin\Release ..\WinRTClientBin\Release /i /s /y
xcopy $SDXROOT\termsrv\CloudDv\Externals\ADAL ..\ADAL /i /s /y
xcopy $SDXROOT\termsrv\rdp\externals\openssl ..\openssl /i /s /y
xcopy $SDXROOT\termsrv\rdp\externals\RdpWinRTTransportRpc ..\RdpWinRTTransportRpc /i /s /y
