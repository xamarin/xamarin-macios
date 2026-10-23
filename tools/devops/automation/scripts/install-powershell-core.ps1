param (
    [string] $Version = $env:POWERSHELL_VERSION,
    [string] $Sha256 = $env:POWERSHELL_ZIP_SHA256,
    [string] $TempDirectory = $env:AGENT_TEMPDIRECTORY
)

function Get-PowerShellVersion {
    param ([Parameter(Mandatory)] [string] $Path)

    $ErrorActionPreference = 'Stop'
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "PowerShell executable '$Path' does not exist."
    }

    $global:LASTEXITCODE = $null
    $version = & $Path -NoLogo -NoProfile -NonInteractive -Command '$PSVersionTable.PSVersion.ToString()'
    $exitCode = $LASTEXITCODE
    if ($null -eq $exitCode -or $exitCode -ne 0) {
        throw "PowerShell executable '$Path' exited with code $exitCode."
    }
    if ([string]::IsNullOrWhiteSpace("$version")) {
        throw "PowerShell executable '$Path' did not report a version."
    }
    return "$version".Trim()
}

function Install-PowerShellCore {
    param (
        [string] $Version,
        [string] $Sha256,
        [string] $TempDirectory
    )

    $ErrorActionPreference = 'Stop'
    $ProgressPreference = 'SilentlyContinue'
    $downloaded = $false
    $existing = Get-Command -Name pwsh.exe -CommandType Application -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($null -ne $existing) {
        $pwsh = $existing.Path
    } else {
        $pwsh = Join-Path $env:ProgramFiles 'PowerShell/7/pwsh.exe'
        if (-not (Test-Path -LiteralPath $pwsh -PathType Leaf)) {
            if ($Version -notmatch '^\d+\.\d+\.\d+$') {
                throw 'POWERSHELL_VERSION must contain a version such as 7.4.0.'
            }
            if ($Sha256 -notmatch '^[0-9a-fA-F]{64}$') {
                throw 'POWERSHELL_ZIP_SHA256 must contain the pinned archive SHA-256.'
            }
            if ([string]::IsNullOrWhiteSpace($TempDirectory) -or -not (Test-Path -LiteralPath $TempDirectory -PathType Container)) {
                throw 'AGENT_TEMPDIRECTORY must contain an existing temporary directory.'
            }

            # A fresh job-local directory avoids elevation and partially installed machine-wide copies.
            $installDirectory = Join-Path $TempDirectory ("PowerShell-$Version-" + [guid]::NewGuid().ToString('N'))
            $archive = "$installDirectory.zip"
            $url = "https://github.com/PowerShell/PowerShell/releases/download/v$Version/PowerShell-$Version-win-x64.zip"
            Write-Host "Downloading $url to $archive"
            Invoke-WebRequest -Uri $url -OutFile $archive -UseBasicParsing
            $actualHash = (Get-FileHash -LiteralPath $archive -Algorithm SHA256).Hash
            if ($actualHash -ne $Sha256) {
                throw "Unexpected SHA-256 for '$archive': expected $Sha256, got $actualHash."
            }

            Write-Host "Extracting $archive to $installDirectory"
            Expand-Archive -LiteralPath $archive -DestinationPath $installDirectory
            Remove-Item -LiteralPath $archive -Force
            $pwsh = Join-Path $installDirectory 'pwsh.exe'
            $downloaded = $true
        }
    }

    $reportedVersion = Get-PowerShellVersion -Path $pwsh
    if ($downloaded -and $reportedVersion -ne $Version) {
        throw "PowerShell executable '$pwsh' reported version $reportedVersion instead of $Version."
    }

    Write-Host "Using PowerShell $reportedVersion at $pwsh"
    Write-Host "##vso[task.prependpath]$(Split-Path -Parent $pwsh)"
}

if ($MyInvocation.InvocationName -ne '.') {
    Install-PowerShellCore -Version $Version -Sha256 $Sha256 -TempDirectory $TempDirectory
}
