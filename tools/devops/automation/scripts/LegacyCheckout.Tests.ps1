BeforeAll {
    Import-Module "$PSScriptRoot/TestConfiguration.psm1" -Force

    function Get-TemplateStepScript {
        param ([string] $Path, [string] $Name)

        $yaml = (Get-Content -LiteralPath $Path -Raw).Replace("`r`n", "`n")
        $pattern = '(?m)^(?<indent> *)- pwsh: \|\n(?<body>(?:\k<indent>    [^\n]*\n|\n)*)' +
            '\k<indent>  name: ' + [regex]::Escape($Name) + '$'
        $match = [regex]::Match($yaml, $pattern)
        if (-not $match.Success) {
            throw "Unable to find PowerShell step '$Name' in '$Path'."
        }
        $prefix = $match.Groups["indent"].Value + "    "
        return [regex]::Replace($match.Groups["body"].Value, '(?m)^' + [regex]::Escape($prefix), "")
    }

    function New-SourceCheckoutFixture {
        param ([bool] $SupportsSharding, [string] $Validator = "")

        $root = Join-Path $TestDrive ([guid]::NewGuid().ToString("N") + " Source With Spaces")
        $checkout = Join-Path $root "checkout"
        $scripts = Join-Path $checkout "tools/devops/automation/scripts"
        New-Item -Path $scripts -ItemType Directory -Force | Out-Null
        Set-Content (Join-Path $scripts "MaciosCI.psd1") @'
@{
    ModuleVersion = '1.0'
    NestedModules = @('ConfigurationIO.psm1', 'TestConfiguration.psm1')
    FunctionsToExport = '*'
}
'@
        Set-Content (Join-Path $scripts "ConfigurationIO.psm1") @'
function Import-BuildConfiguration {
    param ($ConfigFile)
    return Get-Content -LiteralPath $ConfigFile -Raw | ConvertFrom-Json
}
function Edit-BuildConfiguration {
    param ($ConfigKey, $ConfigValue, $ConfigFile)
    $config = Get-Content -LiteralPath $ConfigFile -Raw | ConvertFrom-Json
    $config | Add-Member -NotePropertyName $ConfigKey -NotePropertyValue $ConfigValue -Force
    Set-Content -LiteralPath $ConfigFile -Value ($config | ConvertTo-Json -Depth 20)
}
Export-ModuleMember -Function Import-BuildConfiguration, Edit-BuildConfiguration
'@
        if ($SupportsSharding) {
            Copy-Item "$PSScriptRoot/TestConfiguration.psm1" (Join-Path $scripts "TestConfiguration.psm1")
        } else {
            Set-Content (Join-Path $scripts "TestConfiguration.psm1") @'
function Get-TestConfiguration { throw 'The legacy generator is not needed when importing matrices.' }
Export-ModuleMember -Function Get-TestConfiguration
'@
        }
        if ($Validator) {
            Set-Content (Join-Path $scripts "test-simulator-sharding.ps1") $Validator
        }
        $workspace = Join-Path $root "workspace"
        $artifact = Join-Path $workspace "macios/build-configuration"
        New-Item -Path $artifact -ItemType Directory -Force | Out-Null
        $legacy = [ordered]@{
            monotouch_ios = [ordered]@{
                LABEL = "monotouch"
                TESTS_LABELS = "--label=skip-all-tests,run-monotouch-tests"
                TEST_STAGE = "simulator_tests"
                LABEL_WITH_PLATFORM = "monotouch_ios"
                TEST_PREFIX = "simulator_testsmonotouch_ios"
                TEST_PLATFORM = "iOS"
                STATUS_CONTEXT = "context"
                TEST_FILTER = "Category != MultiPlatform"
            }
        } | ConvertTo-Json -Depth 10 -Compress
        $configPath = Join-Path $root "configuration.json"
        Set-Content $configPath '{"CURRENT_METADATA":"preserved"}'
        return @{
            Root = $root
            Checkout = $checkout
            Workspace = $workspace
            Artifact = Join-Path $artifact "configuration.json"
            ConfigPath = $configPath
            Legacy = $legacy
        }
    }

    function Invoke-CheckoutTemplate {
        param ($Fixture, [string] $Template, [string] $Step, [string] $Full = "", [string] $Simulator = "")

        if ($Full) {
            Set-Content $Fixture.Artifact (@{ TEST_MATRIX = $Full; SIMULATOR_TEST_MATRIX = $Simulator } |
                ConvertTo-Json -Depth 20)
        }
        $body = Get-TemplateStepScript -Path (Join-Path $PSScriptRoot $Template) -Name $Step
        $body = $body.Replace('$(Pipeline.Workspace)', $Fixture.Workspace)
        $preamble = @'
$ErrorActionPreference = "Stop"
$Env:SYSTEM_DEFAULTWORKINGDIRECTORY = $Env:FIXTURE_ROOT
$Env:BUILD_REPOSITORY_TITLE = "checkout"
$Env:CONFIG_PATH = $Env:FIXTURE_CONFIG
Set-Location -LiteralPath $Env:FIXTURE_CHECKOUT
'@
        $runner = Join-Path $Fixture.Root "run-step.ps1"
        Set-Content $runner ($preamble + "`n" + $body)
        $saved = @{}
        foreach ($name in @("FIXTURE_ROOT", "FIXTURE_CONFIG", "FIXTURE_CHECKOUT")) {
            $saved[$name] = [Environment]::GetEnvironmentVariable($name)
        }
        try {
            $Env:FIXTURE_ROOT = $Fixture.Root
            $Env:FIXTURE_CONFIG = $Fixture.ConfigPath
            $Env:FIXTURE_CHECKOUT = $Fixture.Checkout
            $executable = Join-Path $PSHOME $(if ($IsWindows) { "pwsh.exe" } else { "pwsh" })
            $output = & $executable -NoProfile -File $runner 2>&1
            $exitCode = $LASTEXITCODE
        } finally {
            foreach ($name in $saved.Keys) {
                [Environment]::SetEnvironmentVariable($name, $saved[$name])
            }
        }
        return @{
            ExitCode = $exitCode
            Output = $output -join "`n"
            Config = Get-Content $Fixture.ConfigPath -Raw | ConvertFrom-Json -AsHashtable
        }
    }
}

Describe "Merged templates with older source checkouts" {
    It "retains both unsharded matrices when the checked-out module has no splitter" {
        $fixture = New-SourceCheckoutFixture -SupportsSharding $false
        $result = Invoke-CheckoutTemplate $fixture "../templates/common/load_configuration.yml" "test_matrix" `
            $fixture.Legacy $fixture.Legacy
        $result.ExitCode | Should -Be 0
        $result.Output | Should -Match "Legacy source checkout"
        $result.Config.CURRENT_METADATA | Should -BeExactly "preserved"
        $result.Config.TEST_MATRIX | Should -BeExactly $fixture.Legacy
        $result.Config.SIMULATOR_TEST_MATRIX | Should -BeExactly $fixture.Legacy
    }

    It "expands legacy artifacts only when the checked-out source supports sharding" {
        $fixture = New-SourceCheckoutFixture -SupportsSharding $true
        $result = Invoke-CheckoutTemplate $fixture "../templates/common/load_configuration.yml" "test_matrix" `
            $fixture.Legacy $fixture.Legacy
        $result.ExitCode | Should -Be 0
        $full = $result.Config.TEST_MATRIX | ConvertFrom-Json -AsHashtable
        $simulator = $result.Config.SIMULATOR_TEST_MATRIX | ConvertFrom-Json -AsHashtable
        $full.Count | Should -Be 3
        ($full | ConvertTo-Json -Compress) | Should -BeExactly ($simulator | ConvertTo-Json -Compress)
    }

    It "rejects a sharded reporting matrix with an older xharness source checkout" {
        $fixture = New-SourceCheckoutFixture -SupportsSharding $false
        $sharded = Split-SimulatorTestMatrix -TestMatrix $fixture.Legacy
        $result = Invoke-CheckoutTemplate $fixture "../templates/common/load_configuration.yml" "test_matrix" `
            $sharded $fixture.Legacy
        $result.ExitCode | Should -Not -Be 0
        $result.Output | Should -Match "cannot run sharded test artifacts"
        $result.Config.Contains("TEST_MATRIX") | Should -BeFalse
    }

    It "also rejects a sharded execution matrix with an older source checkout" {
        $fixture = New-SourceCheckoutFixture -SupportsSharding $false
        $sharded = Split-SimulatorTestMatrix -TestMatrix $fixture.Legacy
        $result = Invoke-CheckoutTemplate $fixture "../templates/common/load_configuration.yml" "test_matrix" `
            $fixture.Legacy $sharded
        $result.ExitCode | Should -Not -Be 0
        $result.Output | Should -Match "cannot run sharded test artifacts"
    }

    It "rejects shard CLI options even when shard metadata is missing" {
        $fixture = New-SourceCheckoutFixture -SupportsSharding $false
        $matrix = $fixture.Legacy | ConvertFrom-Json -AsHashtable
        $matrix.monotouch_ios.TESTS_LABELS += " --test-variation-shard=1/3"
        $inputJson = $matrix | ConvertTo-Json -Depth 10 -Compress
        $result = Invoke-CheckoutTemplate $fixture "../templates/common/load_configuration.yml" "test_matrix" `
            $inputJson $inputJson
        $result.ExitCode | Should -Not -Be 0
        $result.Output | Should -Match "cannot run sharded test artifacts"
    }

    It "handles an older checkout without the newer adapter validator explicitly" {
        $fixture = New-SourceCheckoutFixture -SupportsSharding $false
        $result = Invoke-CheckoutTemplate $fixture "../templates/build/linux-build-verification.yml" "validate_shards"
        $result.ExitCode | Should -Be 0
        $result.Output | Should -Match "Legacy source checkout"
    }

    It "fails if a sharding-capable checkout lost its validator" {
        $fixture = New-SourceCheckoutFixture -SupportsSharding $true
        $result = Invoke-CheckoutTemplate $fixture "../templates/build/linux-build-verification.yml" "validate_shards"
        $result.ExitCode | Should -Not -Be 0
        $result.Output | Should -Match "validator is missing"
    }

    It "runs the validator present in the source checkout" {
        $fixture = New-SourceCheckoutFixture -SupportsSharding $true -Validator 'Write-Host "VALIDATOR_EXECUTED"'
        $result = Invoke-CheckoutTemplate $fixture "../templates/build/linux-build-verification.yml" "validate_shards"
        $result.ExitCode | Should -Be 0
        $result.Output | Should -Match "VALIDATOR_EXECUTED"
    }

    It "does not turn a validator failure into legacy compatibility success" {
        $fixture = New-SourceCheckoutFixture -SupportsSharding $true -Validator 'throw "Fixture validator failed"'
        $result = Invoke-CheckoutTemplate $fixture "../templates/build/linux-build-verification.yml" "validate_shards"
        $result.ExitCode | Should -Not -Be 0
        $result.Output | Should -Match "Fixture validator failed"
        $result.Output | Should -Not -Match "Legacy source checkout"
    }
}
