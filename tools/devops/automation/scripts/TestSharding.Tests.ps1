using module ".\\TestResults.psm1"

BeforeAll {
    Import-Module "$PSScriptRoot/TestConfiguration.psm1" -Force

    function New-LegacySimulatorMatrix {
        $matrix = [ordered]@{}
        foreach ($label in @("monotouch", "dotnettests")) {
            foreach ($platform in @("iOS", "tvOS", "macOS", "MacCatalyst")) {
                $title = "$($label)_$($platform.ToLower())"
                $matrix[$title] = [ordered]@{
                    LABEL = $label
                    TESTS_LABELS = "--label=skip-all-tests,run-$label-tests"
                    TEST_STAGE = "simulator_tests"
                    LABEL_WITH_PLATFORM = $title
                    TEST_PREFIX = "simulator_tests$title"
                    STATUS_CONTEXT = "context - $title"
                    TEST_PLATFORM = $platform
                    TEST_FILTER = "Category != MultiPlatform"
                    EXTRA_VARIABLE = "preserved"
                }
            }
        }
        $matrix["dotnettests_multiple"] = [ordered]@{
            LABEL = "dotnettests"
            TEST_STAGE = "simulator_tests"
            LABEL_WITH_PLATFORM = "dotnettests_multiple"
            TEST_PREFIX = "simulator_testsdotnettests_multiple"
            TEST_PLATFORM = ""
            TEST_FILTER = "Category = MultiPlatform"
        }
        $matrix["cecil"] = [ordered]@{
            LABEL = "cecil"
            TEST_STAGE = "simulator_tests"
            LABEL_WITH_PLATFORM = "cecil"
            TEST_PREFIX = "simulator_testscecil"
            TEST_PLATFORM = ""
        }
        $matrix["windows"] = [ordered]@{
            LABEL = "windows"
            TEST_STAGE = "windows_integration"
            LABEL_WITH_PLATFORM = "windows"
            TEST_PREFIX = "windows_integrationwindows"
            TEST_PLATFORM = ""
        }
        return $matrix
    }

    function New-ShardedReportFixture {
        param ([string] $Directory)

        $matrix = Split-SimulatorTestMatrix -TestMatrix (New-LegacySimulatorMatrix | ConvertTo-Json -Depth 10) `
            -StageFilter "simulator_tests" | ConvertFrom-Json -AsHashtable
        $outputs = @{}
        foreach ($title in $matrix.Keys) {
            $entry = $matrix[$title]
            $outputs["$title.initialize.TESTS_LABEL"] = $entry.LABEL
            $outputs["$title.initialize.TESTS_TITLE"] = $title
            $outputs["$title.initialize.TESTS_PLATFORM"] = $entry.TEST_PLATFORM
            $outputs["$title.initialize.TESTS_ATTEMPT"] = "1"
            $outputs["$title.initialize.TESTS_BOT"] = "test-bot"
            $outputs["$title.initialize.TESTS_JOBSTATUS"] = "Failed"
            $outputs["$title.runTests.TESTS_JOBSTATUS"] = "Succeeded"
            $artifact = Join-Path $Directory "TestSummary-$($entry.TEST_PREFIX)-1"
            New-Item -Path $artifact -ItemType Directory -Force | Out-Null
            Set-Content -LiteralPath (Join-Path $artifact "TestSummary.md") -Value "# :tada: All 2 tests passed :tada:"
        }
        $dependencies = @{
            configure_build = @{
                configure = @{ outputs = @{ "test_matrix.TEST_MATRIX" = ($matrix | ConvertTo-Json -Depth 10 -Compress) } }
            }
            simulator_tests = @{ tests = @{ outputs = $outputs } }
        }
        return @{ Matrix = $matrix; Dependencies = $dependencies; Outputs = $outputs; Directory = $Directory }
    }

    function Get-ShardedReport {
        param ($Fixture)

        return New-ParallelTestsResults -Path $Fixture.Directory `
            -StageDependencies ($Fixture.Dependencies | ConvertTo-Json -Depth 20 -Compress) `
            -Context "context" -VSDropsIndex "https://drops"
    }

    function Get-ReportComment {
        param ($Report)

        $builder = [System.Text.StringBuilder]::new()
        $Report.WriteComment($builder)
        return $builder.ToString()
    }
}

Describe "Simulator matrix sharding" {
    It "splits exactly the eight targeted platform jobs and preserves other entries" {
        $legacy = New-LegacySimulatorMatrix
        $matrix = Split-SimulatorTestMatrix -TestMatrix ($legacy | ConvertTo-Json -Depth 10) |
            ConvertFrom-Json -AsHashtable

        $matrix.Count | Should -Be 27
        @($matrix.Values | Where-Object { $_.Contains("TEST_SHARD") }).Count | Should -Be 24
        foreach ($title in @("cecil", "windows", "dotnettests_multiple")) {
            ($matrix[$title] | ConvertTo-Json) | Should -BeExactly ($legacy[$title] | ConvertTo-Json)
        }
        foreach ($title in $matrix.Keys | Where-Object { $matrix[$_].Contains("TEST_SHARD") }) {
            $entry = $matrix[$title]
            $entry.LABEL_WITH_PLATFORM | Should -BeExactly $title
            $entry.TEST_PREFIX | Should -BeExactly "$($entry.TEST_STAGE)$title"
            $entry.DISPLAY_NAME | Should -Not -BeNullOrEmpty
            $entry.EXTRA_VARIABLE | Should -BeExactly "preserved"
            $entry.TEST_FILTER | Should -Match "MultiPlatform"
            $title | Should -Match "^[a-z][a-z0-9_]*$"
        }
        @($matrix.Values.TEST_PREFIX | Select-Object -Unique).Count | Should -Be $matrix.Count
    }

    It "is idempotent for configurations imported from an already-sharded build" {
        $once = Split-SimulatorTestMatrix -TestMatrix (New-LegacySimulatorMatrix | ConvertTo-Json -Depth 10)
        Split-SimulatorTestMatrix -TestMatrix $once | Should -BeExactly $once
    }

    It "derives the simulator execution matrix from the full reporting matrix" {
        $full = Split-SimulatorTestMatrix -TestMatrix (New-LegacySimulatorMatrix | ConvertTo-Json -Depth 10)
        $simulator = Split-SimulatorTestMatrix -TestMatrix $full -StageFilter "simulator_tests" |
            ConvertFrom-Json -AsHashtable
        $simulator.Count | Should -Be 26
        $simulator.Contains("windows") | Should -BeFalse
        $fullMatrix = $full | ConvertFrom-Json -AsHashtable
        foreach ($title in $simulator.Keys) {
            ($simulator[$title] | ConvertTo-Json) | Should -BeExactly ($fullMatrix[$title] | ConvertTo-Json)
        }
    }

    It "preserves the original label selection and adds one separate variation-shard option" {
        $legacy = New-LegacySimulatorMatrix
        $matrix = Split-SimulatorTestMatrix -TestMatrix ($legacy | ConvertTo-Json -Depth 10) |
            ConvertFrom-Json -AsHashtable
        foreach ($index in 1..3) {
            $matrix["monotouch_ios_config_$index"].TESTS_LABELS |
                Should -BeExactly "$($legacy.monotouch_ios.TESTS_LABELS) --test-variation-shard=$index/3"
        }
        $matrix.dotnettests_ios_projects.TESTS_LABELS | Should -BeExactly $legacy.dotnettests_ios.TESTS_LABELS
        $matrix.dotnettests_ios_projects.TEST_FILTER |
            Should -BeExactly "(Category != MultiPlatform) & (FullyQualifiedName~Xamarin.Tests.DotNetProjectTest.)"
        $packaging = @(
            "FullyQualifiedName~Xamarin.Tests.AppIconTest.",
            "FullyQualifiedName~Xamarin.Tests.BundleStructureTest.",
            "FullyQualifiedName~Xamarin.Tests.PostBuildTest.",
            "FullyQualifiedName~Xamarin.Tests.PrepareAssembliesTest."
        ) -join " | "
        $other = @(
            "FullyQualifiedName!~Xamarin.Tests.DotNetProjectTest.",
            "FullyQualifiedName!~Xamarin.Tests.AppIconTest.",
            "FullyQualifiedName!~Xamarin.Tests.BundleStructureTest.",
            "FullyQualifiedName!~Xamarin.Tests.PostBuildTest.",
            "FullyQualifiedName!~Xamarin.Tests.PrepareAssembliesTest."
        ) -join " & "
        $expectedPackaging = "(Category != MultiPlatform) & (($packaging) & " +
            "(FullyQualifiedName!~Xamarin.Tests.DotNetProjectTest.))"
        $matrix.dotnettests_ios_packaging.TEST_FILTER | Should -BeExactly $expectedPackaging
        $matrix.dotnettests_ios_other.TEST_FILTER | Should -BeExactly "(Category != MultiPlatform) & ($other)"
    }

    It "generates shards only for enabled platforms and keeps Multiple unsharded" {
        $configurations = @(
            @{ label = "monotouch"; splitByPlatforms = $true; testPrefix = "simulator_tests" },
            @{ label = "dotnettests"; splitByPlatforms = $true; needsMultiplePlatforms = $true; testPrefix = "simulator_tests" }
        ) | ConvertTo-Json
        $platforms = @("iOS", "tvOS", "macOS", "MacCatalyst", "Multiple") |
            ForEach-Object { @{ platform = $_ } } | ConvertTo-Json
        foreach ($enabled in @("", "iOS", "iOS tvOS", "iOS tvOS macOS MacCatalyst")) {
            $matrix = Get-TestConfiguration -TestConfigurations $configurations -SupportedPlatforms $platforms `
                -EnabledPlatforms $enabled -TestsLabels "--label=skip-all-tests" -StatusContext "context" |
                ConvertFrom-Json -AsHashtable
            $enabledCount = @($enabled.Split(' ', [StringSplitOptions]::RemoveEmptyEntries)).Count
            $expected = $enabledCount * 6 + [int]($enabledCount -gt 1)
            $matrix.Count | Should -Be $expected
            $matrix.Contains("dotnettests_multiple") | Should -Be ($enabledCount -gt 1)
        }
    }

    It "rejects an incomplete previously-sharded matrix instead of losing coverage" {
        $matrix = Split-SimulatorTestMatrix -TestMatrix (New-LegacySimulatorMatrix | ConvertTo-Json -Depth 10) |
            ConvertFrom-Json -AsHashtable
        $matrix.Remove("monotouch_ios_config_2")
        { Split-SimulatorTestMatrix -TestMatrix ($matrix | ConvertTo-Json -Depth 10) } |
            Should -Throw "*Expected all three test shards*"
    }

    It "rejects inconsistent artifact identities" {
        $matrix = New-LegacySimulatorMatrix
        $matrix.monotouch_ios.LABEL_WITH_PLATFORM = "wrong_title"
        { Split-SimulatorTestMatrix -TestMatrix ($matrix | ConvertTo-Json -Depth 10) } |
            Should -Throw "*Inconsistent reporting identity*"
    }

    It "rejects malformed matrix JSON" {
        { Split-SimulatorTestMatrix -TestMatrix "[]" } | Should -Throw
        { Split-SimulatorTestMatrix -TestMatrix '{"monotouch_ios": "not an entry"}' } | Should -Throw
    }
}

Describe "Publishing simulator shards" {
    BeforeEach {
        $fixture = New-ShardedReportFixture -Directory (Join-Path $TestDrive "reports")
        $title = "monotouch_ios_config_2"
        $summary = Join-Path $fixture.Directory "TestSummary-simulator_tests$title-1/TestSummary.md"
    }

    It "counts every shard and gives every row a distinct name and artifact link" {
        $report = Get-ShardedReport $fixture
        $report.IsSuccess() | Should -BeTrue
        $report.Results.Count | Should -Be 26
        $report.GetTotalTestCount() | Should -BeExactly ":tada: All 52 tests passed :tada:"
        $comment = Get-ReportComment $report
        foreach ($index in 1..3) {
            $comment.Contains("monotouch - configurations $index/3 (iOS)") | Should -BeTrue
            $comment.Contains("HtmlReport-simulator_testsmonotouch_ios_config_$index-1") | Should -BeTrue
            $comment.Contains("simulator_testsmonotouch_ios_config_$index-1/;/tests/vsdrops_index.html") | Should -BeTrue
        }
        $comment.Contains("dotnettests - projects (macOS)") | Should -BeTrue
        $comment.Contains("dotnettests (Multiple platforms)") | Should -BeTrue
    }

    It "fails and identifies one failing shard without hiding the successful siblings" {
        $fixture.Outputs.Remove("$title.runTests.TESTS_JOBSTATUS")
        Set-Content -LiteralPath $summary -Value "<details>`n<summary>1 tests failed, 1 tests passed.</summary>`n</details>"
        $report = Get-ShardedReport $fixture
        $report.IsSuccess() | Should -BeFalse
        $report.GetFailingTests().Count | Should -Be 1
        $comment = Get-ReportComment $report
        $comment.Contains("### :x: monotouch - configurations 2/3 tests (iOS)") | Should -BeTrue
        $comment.Contains("monotouch - configurations 1/3 (iOS): All 2 tests passed.") | Should -BeTrue
        $comment.Contains("monotouch - configurations 3/3 (iOS): All 2 tests passed.") | Should -BeTrue
    }

    It "fails if a shard has no job outputs even when its summary exists" {
        foreach ($key in @($fixture.Outputs.Keys | Where-Object { $_.StartsWith("$title.") })) {
            $fixture.Outputs.Remove($key)
        }
        $report = Get-ShardedReport $fixture
        $report.IsSuccess() | Should -BeFalse
        $report.GetFailingTests().Count | Should -Be 1
        (Get-ReportComment $report).Contains("Failed catastrophically") | Should -BeTrue
    }

    It "fails if a successful shard has no summary" {
        Remove-Item -LiteralPath $summary
        $report = Get-ShardedReport $fixture
        $report.IsSuccess() | Should -BeFalse
        $report.GetFailingTests().Count | Should -Be 1
        (Get-ReportComment $report).Contains("Failed catastrophically") | Should -BeTrue
    }

    It "does not use a stale attempt-one summary for a retried shard" {
        $fixture.Outputs["$title.initialize.TESTS_ATTEMPT"] = "2"
        $report = Get-ShardedReport $fixture
        $report.IsSuccess() | Should -BeFalse
        $comment = Get-ReportComment $report
        $comment.Contains("[attempt 2]") | Should -BeTrue
        $comment.Contains("HtmlReport-simulator_tests$title-2") | Should -BeTrue
    }

    It "uses the successful retry's own summary and links" {
        $fixture.Outputs["$title.initialize.TESTS_ATTEMPT"] = "2"
        $artifact = Join-Path $fixture.Directory "TestSummary-simulator_tests$title-2"
        New-Item -Path $artifact -ItemType Directory -Force | Out-Null
        Set-Content -LiteralPath (Join-Path $artifact "TestSummary.md") -Value "# :tada: All 3 tests passed :tada:"
        $report = Get-ShardedReport $fixture
        $report.IsSuccess() | Should -BeTrue
        $report.GetTotalTestCount() | Should -BeExactly ":tada: All 53 tests passed :tada:"
        $comment = Get-ReportComment $report
        $comment.Contains("[attempt 2]") | Should -BeTrue
        $comment.Contains("simulator_tests$title-2/;/tests/vsdrops_index.html") | Should -BeTrue
    }

    It "falls back to the correct shard artifact when its VSDrops upload fails" {
        $fixture.Outputs["$title.setVSDropsPublishResult.VSDROPS_PUBLISHED"] = "Failed"
        $report = Get-ShardedReport $fixture
        $report.IsSuccess() | Should -BeTrue
        $comment = Get-ReportComment $report
        $comment.Contains("HtmlReport-simulator_tests$title-1") | Should -BeTrue
        $comment.Contains("simulator_tests$title-1/;/tests/vsdrops_index.html") | Should -BeFalse
        $comment.Contains("Html Report Publish failed") | Should -BeTrue
    }
}
