class TestConfiguration {
    [object] $testConfigurations
    [object] $supportedPlatforms
    [string] $testsLabels
    [string] $statusContext
    [string] $testPrefix

    TestConfiguration (
        [object] $testConfigurations,
        [object] $supportedPlatforms,
        [string] $testsLabels,
        [string] $statusContext,
        [string] $testPrefix) {
        $this.testConfigurations = $testConfigurations
        $this.supportedPlatforms = $supportedPlatforms
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
                foreach ($platformConfig in $this.supportedPlatforms) {
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
                    Write-Host "Running $($platform): runThisPlatform=$runThisPlatform containsDotNetTests: $containsDotNetTests isDotNetPlatform: $isDotNetPlatform containsLegacyTests: $containsLegacyTests isLegacyPlatform: $isLegacyPlatform"
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
                    $platformVars["TEST_PREFIX"] = "$($this.testPrefix)$($underscoredLabel)_$($platform)"
                    if ($platform -eq "Multiple") {
                        $platformVars["TEST_PLATFORM"] = ""
                        $platformVars["TEST_CATEGORY"] = "MultiPlatform"
                    } else {
                        $platformVars["TEST_PLATFORM"] = $platform
                        $platformVars["TEST_CATEGORY"] = "!MultiPlatform"
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

        [string]
        $TestsLabels,

        [string]
        $StatusContext,

        [string]
        $TestPrefix
    )

    $objTestConfigurations = ConvertFrom-Json -InputObject $TestConfigurations
    $objSupportedPlatforms = ConvertFrom-Json -InputObject $SupportedPlatforms

    $config = [TestConfiguration]::new($objTestConfigurations, $objSupportedPlatforms, $TestsLabels, $StatusContext, $TestPrefix)
    return $config.Create()
}

# export public functions, other functions are private and should not be used outside the module.
Export-ModuleMember -Function Get-TestConfiguration
