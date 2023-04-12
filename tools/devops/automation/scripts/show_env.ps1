Write-Host "IsMacOS: ${IsMacOS}"
Write-Host "IsWindows: ${IsWindows}"
Write-Host "IsLinux: ${IsLinux}"

if ($IsMacOS -or $IsLinux) {
    Write-Host "HOSTNAME: $(hostname)"
} else {
    Write-Host "COMPUTERNAME: ${env:COMPUTERNAME}"
}

gci env: | format-table -autosize -wrap

if ($IsMacOS) {
    system_profiler SPSoftwareDataType SPHardwareDataType SPDeveloperToolsDataType
}

