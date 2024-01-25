class TestConfiguration {
    [object] $testConfigurations
    [object] $supportedPlatforms
    [string[]] $enabledPlatforms
    [string] $testsLabels
    [string] $statusContext
    [string] $testPrefix

    TestConfiguration (
        [object] $testConfigurations,
        [object] $supportedPlatforms,
        [string[]] $enabledPlatforms,
        [string] $testsLabels,
        [string] $statusContext,
        [string] $testPrefix) {
        $this.testConfigurations = $testConfigurations
        $this.supportedPlatforms = $supportedPlatforms
        $this.enabledPlatforms = $enabledPlatforms
        $this.testsLabels = $testsLabels
        $this.statusContext = $statusContext
        $this.testPrefix = $testPrefix
    }

    [string] Create() {
        $rv = [ordered]@{}
        foreach ($config in $this.testConfigurations) {
            $label = $config.label
            $underscoredLabel = $label.Replace('-','_')
            $splitByPlatforms = $config.splitByPlatforms

            $vars = [ordered]@{}
            # set common variables
            $vars["LABEL"] = $label
            $vars["TESTS_LABELS"] = "$($this.testsLabels),run-$($label)-tests"
            if ($splitByPlatforms -eq "True") {
                if ($this.enabledPlatforms.Length -eq 0) {
                    Write-Host "No enabled platforms, skipping $label"
                    continue
                }
                if ($this.enabledPlatforms.Length -gt 1) {
                    Write-Host "Multiple platform enabled"
                    $this.enabledPlatforms = $this.enabledPlatforms += "Multiple"
                }
                foreach ($platform in $this.enabledPlatforms) {
                    Write-Host "platform: $platform"
                    $platformConfig = $this.supportedPlatforms | Where-Object { $_.platform -eq $platform }
                    $platform = $platformConfig.platform

                    $runThisPlatform = $false
                    $containsDotNetTests = $($config.containsDotNetTests) -eq "true"
                    $containsLegacyTests = $($config.containsLegacyTests) -eq "true"
                    $isDotNetPlatform = $($platformConfig.isDotNetPlatform) -eq "true"
                    $isLegacyPlatform = $($platformConfig.isLegacyPlatform) -eq "true"
                    if ($containsDotNetTests -and $isDotNetPlatform) {
                        $runThisPlatform = $true
                    } elseif ($containsLegacyTests -and $isLegacyPlatform) {
                        $runThisPlatform = $true
                    }
                    if (!$runThisPlatform) {
                        continue
                    }

                    # create a clone of the general dictionary
                    $platformVars = [ordered]@{}
                    foreach ($pair in $vars.GetEnumerator()) {
                        $platformVars[$pair.Key] = $pair.Value
                    }
                    # set platform-specific variables
                    $platformVars["LABEL_WITH_PLATFORM"] = "$($label)_$($platform)"
                    $platformVars["STATUS_CONTEXT"] = "$($this.statusContext) - $($label) - $($platform)"
                    $platformVars["TEST_PREFIX"] = "$($this.testPrefix)$($underscoredLabel)_$($platform.ToLower())"
                    if ($platform -eq "Multiple") {
                        $platformVars["TEST_PLATFORM"] = ""
                        $platformVars["TEST_FILTER"] = "Category = MultiPlatform"
                    } else {
                        $platformVars["TEST_PLATFORM"] = $platform
                        $platformVars["TEST_FILTER"] = "Category != MultiPlatform"
                    }
                    $platformLabel = "$($label)_$($platform)"
                    $rv[$platformLabel] = $platformVars
                }
            } else {
                # set non-platform specific variables
                $vars["LABEL_WITH_PLATFORM"] = "$label"
                $vars["STATUS_CONTEXT"] = "$($this.statusContext) - $($label)"
                $vars["TEST_PREFIX"] = "$($this.testPrefix)$($underscoredLabel)"
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
        $StatusContext,

        [string]
        $TestPrefix
    )

    $objTestConfigurations = ConvertFrom-Json -InputObject $TestConfigurations
    $objSupportedPlatforms = ConvertFrom-Json -InputObject $SupportedPlatforms
    $arrEnabledPlatforms = -split $EnabledPlatforms
    $config = [TestConfiguration]::new($objTestConfigurations, $objSupportedPlatforms, $arrEnabledPlatforms, $TestsLabels, $StatusContext, $TestPrefix)
    return $config.Create()
}

# export public functions, other functions are private and should not be used outside the module.
Export-ModuleMember -Function Get-TestConfiguration
