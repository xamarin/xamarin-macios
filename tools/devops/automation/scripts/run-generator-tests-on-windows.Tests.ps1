Describe 'Windows generator configuration wiring (<LineEnding>)' -Tag 'Wiring' -ForEach @(
    @{ LineEnding = 'LF'; NewLine = "`n" }
    @{ LineEnding = 'CRLF'; NewLine = "`r`n" }
) {
    BeforeAll {
        $yaml = Get-Content -Raw (Join-Path $PSScriptRoot '../templates/windows/build.yml')
        $yaml = ($yaml -replace '\r?\n', $NewLine).Replace("`r`n", "`n")
        $configurationSteps = @([regex]::Matches($yaml, '(?ms)^- .*?(?=^-|\z)') |
            Where-Object { $_.Value -match '(?m)^  name: configuration$' })
        $configurationStep = ($configurationSteps | ForEach-Object { $_.Value }) -join "`n"
        $generatorStep = [regex]::Match($yaml, '(?ms)^- pwsh: [^\n]*run-generator-tests-on-windows\.ps1.*?(?=^-|\z)').Value
    }

    Describe 'Windows generator configuration outputs' {
        BeforeAll {
            Import-Module (Join-Path $PSScriptRoot 'VSTS.psm1') -Force
        }

        It 'exports every configuration variable consumed by the generator as a named task output' {
            $configuration = @{
                DOTNET_PLATFORMS = 'iOS'
                INCLUDE_DOTNET_IOS = 'true'
                IOS_NUGET_VERSION_NO_METADATA = '1.0'
                IOS_NUGET_SDK_NAME = 'TestSdk'
                IOS_NUGET_REF_NAME = 'TestRef'
                DOTNET_IOS_RUNTIME_IDENTIFIERS = 'ios-arm64'
                'ios-arm64_NUGET_RUNTIME_NAME' = 'TestRuntime'
                XCODE_VERSION = '27.1'
                XCODE_IS_STABLE = 'False'
            }
            $generator = Get-Content -Raw (Join-Path $PSScriptRoot 'run-generator-tests-on-windows.ps1')
            foreach ($read in [regex]::Matches($generator, '\$Env:CONFIGURATION_(\w+)')) {
                $configuration.Keys | Should -Contain $read.Groups[1].Value
            }
            $path = Join-Path $TestDrive 'configuration.json'
            Set-Content -LiteralPath $path -Value (($configuration | ConvertTo-Json) -replace '\r?\n', $NewLine) -NoNewline

            $records = @(Import-BuildConfiguration -ConfigFile $path 6>&1)
            $messages = @($records | Where-Object { $_ -is [System.Management.Automation.InformationRecord] } |
                ForEach-Object { $_.MessageData.ToString() })
            foreach ($entry in $configuration.GetEnumerator()) {
                $messages | Should -Contain "##vso[task.setvariable variable=$($entry.Key);isOutput=true]$($entry.Value)"
            }
        }

        It 'rejects missing configuration instead of providing empty defaults' {
            { Import-BuildConfiguration -ConfigFile (Join-Path $TestDrive 'missing.json') } |
                Should -Throw '*Configuration file*is missing*'
        }

        It 'rejects null configuration instead of providing empty defaults' {
            $path = Join-Path $TestDrive 'null.json'
            Set-Content -LiteralPath $path -Value 'null'
            { Import-BuildConfiguration -ConfigFile $path } |
                Should -Throw '*Failed to load configuration file*'
        }
    }

    It 'loads the named configuration outputs before running the generator' {
        $configurationSteps.Count | Should -Be 1
        $configurationStep | Should -Match 'Import-BuildConfiguration -ConfigFile \$configFile'
        $configurationStep | Should -Match 'build-configuration\\+configuration\.json'
        $generatorStep | Should -Not -BeNullOrEmpty
        $yaml.IndexOf('run-remote-windows-tests.ps1') | Should -BeGreaterOrEqual 0
        $yaml.IndexOf('run-remote-windows-tests.ps1') | Should -BeLessThan $yaml.IndexOf($configurationStep)
        $yaml.IndexOf($configurationStep) | Should -BeLessThan $yaml.IndexOf($generatorStep)
    }

    It 'does not let the generator continue after configuration loading fails' {
        $configurationStep | Should -Not -Match 'continueOnError'
        $generatorStep | Should -Not -Match 'continueOnError'
        $configurationStep | Should -Not -Match '(?m)^  condition:'
        $generatorStep | Should -Not -Match '(?m)^  condition:'
    }

    It 'allows ten minutes for Windows task startup and configuration loading' {
        $timeout = [regex]::Match($configurationStep, '(?m)^  timeoutInMinutes: (\d+)$')
        $timeout.Success | Should -BeTrue
        [int] $timeout.Groups[1].Value | Should -Be 10
    }
}
