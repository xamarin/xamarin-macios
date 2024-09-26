Write-Host "IsMacOS: ${IsMacOS}"
Write-Host "IsWindows: ${IsWindows}"
Write-Host "IsLinux: ${IsLinux}"

if ($IsMacOS -or $IsLinux) {
    Write-Host "HOSTNAME: $(hostname)"
} else {
    Write-Host "COMPUTERNAME: ${env:COMPUTERNAME}"
}

gci env: | format-table -autosize

gci env: | format-table -autosize | Out-String -Width 8192

gci env: | format-table -autosize -wrap

if ($IsMacOS) {
    Write-Host ""
    Write-Host "## System profile"
    Write-Host ""
    system_profiler SPSoftwareDataType SPHardwareDataType SPDeveloperToolsDataType

    Write-Host ""
    Write-Host "## Network configuration"
    Write-Host ""
    ifconfig | grep 'inet '


    Write-Host ""
    Write-Host "## Top processes"
    Write-Host ""
    top -l 1 -o TIME
}

