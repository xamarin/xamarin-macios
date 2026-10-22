class TestConfiguration {
    [object] $testConfigurations
    [object] $supportedPlatforms
    [string[]] $enabledPlatforms
    [string] $testsLabels
    [string] $statusContext
    [string] $stageFilter

    TestConfiguration (
        [object] $testConfigurations,
        [object] $supportedPlatforms,
        [string[]] $enabledPlatforms,
        [string] $testsLabels,
        [string] $statusContext,
        [string] $stageFilter) {
        $this.testConfigurations = $testConfigurations
        $this.supportedPlatforms = $supportedPlatforms
        $this.enabledPlatforms = $enabledPlatforms
        $this.testsLabels = $testsLabels
        $this.statusContext = $statusContext
        $this.stageFilter = $stageFilter
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

            if ($this.stageFilter) {
                if ($testStage -eq $this.stageFilter) {
                    Write-Host "Test $label with testStage '$testStage' is included, because stage filter is '$this.stageFilter'"
                } else {
                    Write-Host "Test $label with testStage '$testStage' is skipped, because stage filter is '$this.stageFilter'"
                    continue
                }
            } else {
                Write-Host "Test $label with testStage '$testStage' is included, because there's no stage filter set"
            }

            # Skip macOS tests if neither macOS nor MacCatalyst platforms are enabled
            if ($config.isMacTest -eq "true" -or $config.isMacTest -eq $true) {
                $hasMacPlatform = ($this.enabledPlatforms -contains "macOS") -or ($this.enabledPlatforms -contains "MacCatalyst")
                if (-not $hasMacPlatform) {
                    Write-Host "Skipping mac test $label - neither macOS nor MacCatalyst platforms are enabled"
                    continue
                }
            }

            $vars = [ordered]@{}
            # set common variables
            $vars["LABEL"] = $label
            $vars["TESTS_LABELS"] = "$($this.testsLabels),run-$($label)-tests"
            $vars["TEST_STAGE"] = $testStage
            if ($config.displayName) {
                $vars["DISPLAY_NAME"] = $config.displayName
            }
            if ($config.isMacTest -eq "true" -or $config.isMacTest -eq $true) {
                $vars["IS_MAC_TEST"] = "true"
            }

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
                    $platformVars["LABEL_WITH_PLATFORM"] = "$($label)_$($platform.ToLower())"
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
                if ($this.enabledPlatforms.Length -eq 0 -and $config.supportsNoPlatforms -ne "true" -and $config.supportsNoPlatforms -ne $true) {
                    Write-Host "No enabled platforms, skipping test $label (supportsNoPlatforms=$($config.supportsNoPlatforms))"
                    continue
                }
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
        $StatusContext,

        [string]
        [AllowEmptyString()]
        $StageFilter
    )

    $objTestConfigurations = ConvertFrom-Json -InputObject $TestConfigurations
    $objSupportedPlatforms = ConvertFrom-Json -InputObject $SupportedPlatforms
    $arrEnabledPlatforms = -split $EnabledPlatforms | Where { $_ }
    $config = [TestConfiguration]::new($objTestConfigurations, $objSupportedPlatforms, $arrEnabledPlatforms, $TestsLabels, $StatusContext, $StageFilter)
    return Split-SimulatorTestMatrix -TestMatrix $config.Create()
}

function Split-SimulatorTestMatrix {
    param (
        [Parameter(Mandatory)]
        [string]
        $TestMatrix,

        [string]
        $StageFilter = ""
    )

    $matrix = ConvertFrom-Json -InputObject $TestMatrix -AsHashtable -ErrorAction Stop
    if ($matrix -isnot [System.Collections.IDictionary]) {
        throw "The test matrix must be a JSON object."
    }

    $projectFilter = "FullyQualifiedName~Xamarin.Tests.DotNetProjectTest."
    $packagingClasses = @("AppIconTest", "BundleStructureTest", "PostBuildTest", "PrepareAssembliesTest")
    $packagingFilter = ($packagingClasses | ForEach-Object { "FullyQualifiedName~Xamarin.Tests.$($_)." }) -join " | "
    $otherFilters = @($projectFilter.Replace("~", "!~")) +
        @($packagingClasses | ForEach-Object { "FullyQualifiedName!~Xamarin.Tests.$($_)." })
    $dotnetShards = @(
        @{ Name = "projects"; DisplayName = "projects"; Filter = $projectFilter },
        @{ Name = "packaging"; DisplayName = "packaging"; Filter = "($packagingFilter) & ($($otherFilters[0]))" },
        @{ Name = "other"; DisplayName = "other"; Filter = $otherFilters -join " & " }
    )
    $variationShards = @(1..3 | ForEach-Object {
        @{ Name = "config_$_"; DisplayName = "configurations $_/3"; Arguments = "--test-variation-shard=$_/3" }
    })

    $result = [ordered]@{}
    $groups = @{}
    foreach ($title in $matrix.Keys) {
        $entry = $matrix[$title]
        if ($entry -isnot [System.Collections.IDictionary]) {
            throw "Test matrix entry '$title' must be a JSON object."
        }
        if ($StageFilter -and $entry.TEST_STAGE -ne $StageFilter) {
            continue
        }
        if ($entry.TEST_STAGE -ne "simulator_tests" -or
            $entry.LABEL -notin @("monotouch", "dotnettests") -or
            $entry.TEST_PLATFORM -notin @("iOS", "tvOS", "macOS", "MacCatalyst")) {
            $result.Add($title, $entry)
            continue
        }

        $shards = if ($entry.LABEL -eq "monotouch") { $variationShards } else { $dotnetShards }
        $group = "$($entry.LABEL)/$($entry.TEST_PLATFORM)"
        if (-not $groups.ContainsKey($group)) {
            $groups[$group] = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
        }
        if ($entry.LABEL_WITH_PLATFORM -cne $title -or $entry.TEST_PREFIX -cne "$($entry.TEST_STAGE)$title") {
            throw "Inconsistent reporting identity for test matrix entry '$title'."
        }
        if ($entry.Contains("TEST_SHARD")) {
            if ($entry.TEST_SHARD -notin $shards.Name -or -not $groups[$group].Add($entry.TEST_SHARD)) {
                throw "Invalid or duplicate shard '$($entry.TEST_SHARD)' for '$group'."
            }
            $result.Add($title, $entry)
            continue
        }

        foreach ($shard in $shards) {
            if (-not $groups[$group].Add($shard.Name)) {
                throw "Duplicate shard '$($shard.Name)' for '$group'."
            }
            $variables = [ordered]@{}
            foreach ($pair in $entry.GetEnumerator()) {
                $variables[$pair.Key] = $pair.Value
            }
            $shardTitle = "${title}_$($shard.Name)"
            $variables["LABEL_WITH_PLATFORM"] = $shardTitle
            $variables["TEST_PREFIX"] = "$($entry.TEST_STAGE)$shardTitle"
            $variables["STATUS_CONTEXT"] = "$($entry.STATUS_CONTEXT) - $($shard.DisplayName)"
            $variables["DISPLAY_NAME"] = "$($entry.LABEL) - $($shard.DisplayName)"
            $variables["TEST_SHARD"] = $shard.Name
            if ($shard.Arguments) {
                $variables["TESTS_LABELS"] = "$($entry.TESTS_LABELS) $($shard.Arguments)"
            }
            if ($shard.Filter) {
                $variables["TEST_FILTER"] = if ($entry.TEST_FILTER) {
                    "($($entry.TEST_FILTER)) & ($($shard.Filter))"
                } else {
                    $shard.Filter
                }
            }
            $result.Add($shardTitle, $variables)
        }
    }

    foreach ($group in $groups.Keys) {
        if ($groups[$group].Count -ne 3) {
            throw "Expected all three test shards for '$group', found $($groups[$group].Count)."
        }
    }

    return $result | ConvertTo-Json -Depth 10
}

# export public functions, other functions are private and should not be used outside the module.
Export-ModuleMember -Function Get-TestConfiguration, Split-SimulatorTestMatrix
