param (
    [string] $DotNetPath = ""
)

$ErrorActionPreference = "Stop"
Import-Module "$PSScriptRoot/TestConfiguration.psm1"

if (-not $DotNetPath) {
    $repository = (Resolve-Path "$PSScriptRoot/../../../..").Path
    $executable = if ($IsWindows) { "dotnet.exe" } else { "dotnet" }
    $DotNetPath = Join-Path $repository "builds/downloads/dotnet/$executable"
}
if (-not (Test-Path -LiteralPath $DotNetPath -PathType Leaf)) {
    throw "The .NET SDK was not found at '$DotNetPath'. Run 'make -C builds dotnet' or pass -DotNetPath."
}

$project = Join-Path $PSScriptRoot "test_data/NUnitShardFixture/NUnitShardFixture.csproj"
$configurations = ConvertTo-Json -InputObject @(
    @{ label = "dotnettests"; splitByPlatforms = $true; testPrefix = "simulator_tests" }
)
$platforms = ConvertTo-Json -InputObject @(@{ platform = "iOS" })
$matrix = Get-TestConfiguration -TestConfigurations $configurations -SupportedPlatforms $platforms `
    -EnabledPlatforms "iOS" -TestsLabels "--label=skip-all-tests" -StatusContext "shard regression" |
    ConvertFrom-Json -AsHashtable
if ($matrix.Count -ne 3) {
    throw "Expected three integration-test shards, found $($matrix.Count)."
}
foreach ($shard in @("projects", "packaging", "other")) {
    if (-not $matrix.Contains("dotnettests_ios_$shard")) {
        throw "The '$shard' integration-test shard is missing."
    }
}

function Get-DiscoveredTests {
    param ([string] $Filter)

    $arguments = @(
        "test", $project, "--no-build", "--no-restore", "--list-tests", "--verbosity", "quiet",
        "--filter", $Filter
    )
    $output = & $DotNetPath @arguments 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "NUnit discovery failed for filter '$Filter':`n$($output -join "`n")"
    }

    $tests = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    $listing = $false
    foreach ($line in $output) {
        $text = $line.ToString()
        if ($text.Trim() -eq "The following Tests are available:") {
            $listing = $true
        } elseif ($listing -and $text.StartsWith("    ")) {
            $name = $text.Trim()
            if (-not $tests.Add($name)) {
                throw "The adapter fixture contains a duplicate displayed test name: '$name'."
            }
        }
    }
    if (-not $listing -or $tests.Count -eq 0) {
        throw "No NUnit cases were discovered for filter '$Filter':`n$($output -join "`n")"
    }
    return ,$tests
}

$previousLanguage = $Env:DOTNET_CLI_UI_LANGUAGE
try {
    $Env:DOTNET_CLI_UI_LANGUAGE = "en-US"
    & $DotNetPath build $project --verbosity quiet | Out-Host
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to build the NUnit shard-discovery fixture."
    }

    $all = Get-DiscoveredTests -Filter "(Category != MultiPlatform) & (Category != Windows)"
    if ($all.Count -ne 9 -or $all.Contains("MultiplePlatformsCase") -or $all.Contains("WindowsCase")) {
        throw "Expected all nine fixture cases with the existing platform and Windows exclusions applied."
    }
    $combined = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($title in $matrix.Keys) {
        $filter = "($($matrix[$title].TEST_FILTER)) & (Category != Windows)"
        $selected = Get-DiscoveredTests -Filter $filter
        foreach ($name in $selected) {
            if (-not $combined.Add($name)) {
                throw "NUnit case '$name' is selected by more than one shard."
            }
        }
        Write-Host "$title selected $($selected.Count) cases."
    }
    if (-not $combined.SetEquals($all)) {
        throw "The NUnit shards do not select exactly the same cases as the unsharded filter."
    }
    Write-Host "Validated $($all.Count) NUnit cases: all three shards are disjoint and collectively exhaustive."
} finally {
    $Env:DOTNET_CLI_UI_LANGUAGE = $previousLanguage
}
