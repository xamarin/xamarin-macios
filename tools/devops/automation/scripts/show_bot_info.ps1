Write-Host "IsMacOS: ${IsMacOS}"
Write-Host "IsWindows: ${IsWindows}"
Write-Host "IsLinux: ${IsLinux}"

if ($IsMacOS -or $IsLinux) {
    Write-Host "HOSTNAME: $(hostname)"
} else {
    Write-Host "COMPUTERNAME: ${env:COMPUTERNAME}"
}

Get-ChildItem env: | Sort-Object -Property Name | Format-Table -AutoSize | Out-String -Width 81920

if ($IsMacOS) {
    Write-Host ""
    Write-Host "## Uptime"
    Write-Host ""
    uptime

    Write-Host ""
    Write-Host "## System profile"
    Write-Host ""
    system_profiler SPSoftwareDataType SPHardwareDataType SPDeveloperToolsDataType

    Write-Host ""
    Write-Host "## Network configuration"
    Write-Host ""
    ifconfig | grep 'inet '

    Write-Host ""
    Write-Host "## Top processes (ps)"
    Write-Host ""
    ps aux

    Write-Host ""
    Write-Host "## Python3 location:"
    Write-Host ""
    which python3

    Write-Host ""
    Write-Host "## Pip3 version:"
    Write-Host ""
    pip3 -V

    Write-Host ""
    Write-Host "## Hardware info"
    Write-Host ""
    ioreg -l | grep -e Manufacturer -e 'Vendor Name'
}

