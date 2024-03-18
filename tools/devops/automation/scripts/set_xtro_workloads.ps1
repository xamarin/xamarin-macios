param
(
    [Parameter(Mandatory)]
    [String]
    $WorkloadPath
)

$versionData = Get-Content $WorkloadPath | ConvertFrom-Json
$platforms = $("iOS", "tvOS", "macOS", "MacCatalyst")
foreach ($platform in $platforms) {
    $platformLower = $platform.ToLowerInvariant()
    $platformUpper = $platform.ToUpperInvariant()
    $version = $versionData."microsoft.net.sdk.$platformLower"
    if (![string]::IsNullOrEmpty($version)) {
        $version = $version.Substring(0, $version.IndexOf('/'))
        Write-Host "Platform: $platform has version $version"
        Write-Host "##vso[task.setvariable variable=$($platformUpper)_WORKLOAD_VERSION;]$version"
    } else {
        Write-Host "Platform: $platform has no version (disabled?)"
    }
}
