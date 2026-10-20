param ([string] $FixtureRoot = $env:AGENT_TEMPDIRECTORY)

$ErrorActionPreference = 'Stop'
if ($PSVersionTable.PSEdition -ne 'Desktop' -or $PSVersionTable.PSVersion.Major -ne 5 -or $PSVersionTable.PSVersion.Minor -ne 1) {
    throw 'Run the native bootstrap tests with Windows PowerShell 5.1 (powershell.exe).'
}
if ([string]::IsNullOrWhiteSpace($FixtureRoot) -or -not (Test-Path -LiteralPath $FixtureRoot -PathType Container)) {
    throw 'Set AGENT_TEMPDIRECTORY or pass an existing directory with -FixtureRoot.'
}

. (Join-Path $PSScriptRoot 'install-powershell-core.ps1')

$directory = Join-Path $FixtureRoot ('PowerShell native tests ' + [guid]::NewGuid().ToString('N'))
New-Item -Path $directory -ItemType Directory | Out-Null
$results = foreach ($expectedExitCode in @(1, 0)) {
    $fixture = Join-Path $directory "exit-$expectedExitCode.exe"
    $source = @"
public static class NativeProbe$expectedExitCode {
    public static int Main (string [] args) {
        return $expectedExitCode;
    }
}
"@
    Add-Type -TypeDefinition $source -OutputAssembly $fixture -OutputType ConsoleApplication

    $global:LASTEXITCODE = 1 - $expectedExitCode
    $message = $null
    try {
        $null = Get-PowerShellVersion -Path $fixture
    } catch {
        $message = $_.Exception.Message
    }
    $nativeExitCode = $LASTEXITCODE
    $expectedMessage = if ($expectedExitCode -eq 1) {
        "PowerShell executable '$fixture' exited with code 1."
    } else {
        "PowerShell executable '$fixture' did not report a version."
    }
    if ($nativeExitCode -ne $expectedExitCode -or $message -cne $expectedMessage) {
        throw "Native exit ${expectedExitCode}: expected '$expectedMessage', got exit '$nativeExitCode' and message '$message'."
    }

    [pscustomobject]@{
        ExpectedExitCode = $expectedExitCode
        NativeExitCode = $nativeExitCode
        Message = $message
        Fixture = $fixture
        PowerShellEdition = $PSVersionTable.PSEdition
        PowerShellVersion = $PSVersionTable.PSVersion.ToString()
    }
}
$results | ConvertTo-Json -Compress
exit 0
