class TestConfiguration {
    [object] $testConfigurations
    [object] $supportedPlatforms
    [string[]] $enabledPlatforms
    [string] $testsLabels
    [string] $statusContext

    TestConfiguration (
        [object] $testConfigurations,
        [object] $supportedPlatforms,
        [string[]] $enabledPlatforms,
        [string] $testsLabels,
        [string] $statusContext) {
        $this.testConfigurations = $testConfigurations
        $this.supportedPlatforms = $supportedPlatforms
        $this.enabledPlatforms = $enabledPlatforms
        $this.testsLabels = $testsLabels
        $this.statusContext = $statusContext
    }

    [string] Create() {
        $rv = [ordered]@{}
        foreach ($config in $this.testConfigurations) {
            $enabledPlatformsForConfig = $this.enabledPlatforms
            $label = $config.label
            $underscoredLabel = $label.Replace('-','_')
            $splitByPlatforms = $config.splitByPlatforms
            $testPrefix = $config.testPrefix
            $testStage = $config.testStage ? $config.testStage : $config.testPrefix

            $vars = [ordered]@{}
            # set common variables
            $vars["LABEL"] = $label
            $vars["TESTS_LABELS"] = "$($this.testsLabels),run-$($label)-tests"
            $vars["TEST_STAGE"] = $testStage
            if ($splitByPlatforms -eq "True") {
                if ($enabledPlatformsForConfig.Length -eq 0) {
                    Write-Host "No enabled platforms, skipping $label"
                    continue
                }

                if ($enabledPlatformsForConfig.Length -gt 1) {
                    if ($config.needsMultiplePlatforms -eq "true") {
                        Write-Host "Multiple platform enabled"
                        $enabledPlatformsForConfig += "Multiple"
                    } else {
                        Write-Host "Test $label has multiple platforms, but does not need a specific multiple test run (needsMultiplePlatforms=$($config.needsMultiplePlatforms))."
                    }
                }
                foreach ($platform in $enabledPlatformsForConfig) {
                    Write-Host "platform: $platform"
                    $platformConfig = $this.supportedPlatforms | Where-Object { $_.platform -eq $platform }
                    $platform = $platformConfig.platform

                    $runThisPlatform = $false

                    # create a clone of the general dictionary
                    $platformVars = [ordered]@{}
                    foreach ($pair in $vars.GetEnumerator()) {
                        $platformVars[$pair.Key] = $pair.Value
                    }
                    # set platform-specific variables
                    $platformVars["LABEL_WITH_PLATFORM"] = "$($label)_$($platform)"
                    $platformVars["STATUS_CONTEXT"] = "$($this.statusContext) - $($label) - $($platform)"
                    $platformVars["TEST_PREFIX"] = "$($testPrefix)$($underscoredLabel)_$($platform.ToLower())"
                    if ($platform -eq "Multiple") {
                        $platformVars["TEST_PLATFORM"] = ""
                        $platformVars["TEST_FILTER"] = "Category = MultiPlatform"
                    } else {
                        $platformVars["TEST_PLATFORM"] = $platform
                        $platformVars["TEST_FILTER"] = "Category != MultiPlatform"
                    }
                    $platformLabel = "$($label)_$($platform.ToLower())"
                    $rv[$platformLabel] = $platformVars
                }
            } else {
                # set non-platform specific variables
                $vars["LABEL_WITH_PLATFORM"] = "$label"
                $vars["STATUS_CONTEXT"] = "$($this.statusContext) - $($label)"
                $vars["TEST_PREFIX"] = "$($testPrefix)$($underscoredLabel)"
                $vars["TEST_PLATFORM"] = ""
                $rv[$label] = $vars
            }
        }

        $rv

        return $rv | ConvertTo-Json
    }
}

function Get-TestConfiguration {
    param
    (
        [Parameter(Mandatory)]
        [string]
        $TestConfigurations,

        [Parameter(Mandatory)]
        [string]
        $SupportedPlatforms,

        [Parameter(Mandatory)]
        [string]
        [AllowEmptyString()]
        $EnabledPlatforms,

        $TestsLabels,

        [string]
        $StatusContext
    )

    $objTestConfigurations = ConvertFrom-Json -InputObject $TestConfigurations
    $objSupportedPlatforms = ConvertFrom-Json -InputObject $SupportedPlatforms
    $arrEnabledPlatforms = -split $EnabledPlatforms | Where { $_ }
    $config = [TestConfiguration]::new($objTestConfigurations, $objSupportedPlatforms, $arrEnabledPlatforms, $TestsLabels, $StatusContext)
    return $config.Create()
}

# export public functions, other functions are private and should not be used outside the module.
Export-ModuleMember -Function Get-TestConfiguration
