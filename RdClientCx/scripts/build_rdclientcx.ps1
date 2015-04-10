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
xcopy $SDXROOT\testsrc\termsrvtestdata\REDIST\WinRTClientBin\Debug ..\WinRTClientBin\Debug /i /s /y

BcxPlatform "x86fre"
xcopy $SDXROOT\testsrc\termsrvtestdata\REDIST\WinRTClientBin\Release ..\WinRTClientBin\Release /i /s /y

BcxPlatform "armchk"
xcopy $SDXROOT\termsrv\CloudDv\Externals\ADAL ..\ADAL /i /s /y

BcxPlatform "armfre"
xcopy $SDXROOT\termsrv\rdp\externals\openssl ..\openssl /i /s /y
