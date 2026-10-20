Describe 'PowerShell bootstrap wiring (<LineEnding>)' -Tag 'Wiring' -ForEach @(
    @{ LineEnding = 'LF'; NewLine = "`n" }
    @{ LineEnding = 'CRLF'; NewLine = "`r`n" }
) {
    BeforeAll {
        $yaml = Get-Content -Raw (Join-Path $PSScriptRoot '../templates/common/checkout.yml')
        $yaml = $yaml -replace '\r?\n', $NewLine
        $yaml = $yaml.Replace("`r`n", "`n")
        $installStep = [regex]::Match($yaml, '(?ms)^- powershell: \./install-powershell-core\.ps1.*?(?=^-|\z)').Value
    }

    It 'uses Windows PowerShell to run the bootstrap from the checked-out scripts directory' {
        $installStep | Should -Match '(?m)^- powershell: \./install-powershell-core\.ps1$'
        $installStep | Should -Match '(?m)^  workingDirectory: \$\(System\.DefaultWorkingDirectory\)/\$\(BUILD_REPOSITORY_TITLE\)/tools/devops/automation/scripts$'
    }

    It 'preserves the Windows-only guard and runs before Undo Github merge' {
        $installStep | Should -Match "condition: and\(succeeded\(\), eq\(variables\['Agent.OS'\], 'Windows_NT'\)\)"
        $yaml.IndexOf('- powershell:') | Should -BeLessThan $yaml.IndexOf('- pwsh: ./undo_github_merge.ps1')
        $installStep | Should -Not -Match 'continueOnError'
    }

    It 'does not launch an unchecked machine-wide installer' {
        $installStep | Should -Not -Match 'msiexec|\.msi|exit 0'
    }

    It 'pins the portable archive version and its official checksum' {
        $installStep | Should -Match '(?m)^    POWERSHELL_VERSION: 7\.4\.0$'
        $installStep | Should -Match '(?m)^    POWERSHELL_ZIP_SHA256: 62151db1d98a8b56aeb249cc8a3ce17948f1c83b4062dac8d0c4302de71cbd75$'
        $installStep | Should -Match '(?m)^  timeoutInMinutes: \d+$'
    }

    It 'runs the Windows native checks before Undo can switch to an older PR head' {
        $nativeStep = [regex]::Match($yaml, '(?ms)^- powershell: \./test-powershell-core-native\.ps1.*?(?=^-|\z)').Value
        $nativeStep | Should -Match '(?m)^- powershell: \./test-powershell-core-native\.ps1$'
        $nativeStep | Should -Match '(?m)^  workingDirectory: \$\(System\.DefaultWorkingDirectory\)/\$\(BUILD_REPOSITORY_TITLE\)/tools/devops/automation/scripts$'
        $nativeStep | Should -Match '(?m)^  timeoutInMinutes: \d+$'
        $nativeStep | Should -Match "condition: and\(succeeded\(\), eq\(variables\['Agent.OS'\], 'Windows_NT'\)\)"
        $nativeStep | Should -Not -Match 'continueOnError'
        $yaml.IndexOf('- powershell: ./test-powershell-core-native.ps1') | Should -BeGreaterThan $yaml.IndexOf('- powershell: ./install-powershell-core.ps1')
        $yaml.IndexOf('- powershell: ./test-powershell-core-native.ps1') | Should -BeLessThan $yaml.IndexOf('- pwsh: ./undo_github_merge.ps1')
    }
}

Describe 'Install-PowerShellCore' {
    BeforeAll {
        . (Join-Path $PSScriptRoot 'install-powershell-core.ps1')
        $originalProbe = (Get-Item Function:Get-PowerShellVersion).ScriptBlock
        $savedProgramFiles = $env:ProgramFiles
    }

    BeforeEach {
        $script:events = [System.Collections.Generic.List[string]]::new()
        $script:version = '7.4.0'
        $script:sha256 = '62151db1d98a8b56aeb249cc8a3ce17948f1c83b4062dac8d0c4302de71cbd75'
        $script:temp = Join-Path $TestDrive 'Agent Temp With Spaces'
        New-Item -Path $script:temp -ItemType Directory -Force | Out-Null
        $env:ProgramFiles = Join-Path $TestDrive ("Program Files " + [guid]::NewGuid().ToString('N'))
        $script:standard = Join-Path $env:ProgramFiles 'PowerShell/7/pwsh.exe'
        $script:archive = $null
        $script:destination = $null
        $script:probePath = $null

        Mock Get-Command { $null } -ParameterFilter { $Name -eq 'pwsh.exe' }
        Mock Invoke-WebRequest {
            $script:events.Add('download')
            $script:archive = $OutFile
        }
        Mock Get-FileHash {
            $script:events.Add('hash')
            [pscustomobject]@{ Hash = $script:sha256 }
        }
        Mock Expand-Archive {
            $script:events.Add('expand')
            $script:destination = $DestinationPath
        }
        Mock Remove-Item {} -ParameterFilter { $LiteralPath -like '*.zip' }
        Mock Get-PowerShellVersion {
            $script:events.Add('probe')
            $script:probePath = $Path
            $script:version
        }
        Mock Write-Host { $script:events.Add(($Object -join ' ')) }
    }

    AfterAll {
        $env:ProgramFiles = $savedProgramFiles
    }

    It 'reuses an application on PATH without needing installation settings' {
        $path = Join-Path $TestDrive 'Existing PowerShell/pwsh.exe'
        Mock Get-Command { [pscustomobject]@{ Path = $path } } -ParameterFilter { $Name -eq 'pwsh.exe' }
        Mock Get-PowerShellVersion { '7.6.0' }

        Install-PowerShellCore -Version '' -Sha256 '' -TempDirectory ''

        Should -Invoke Get-Command -Times 1 -Exactly -ParameterFilter { $Name -eq 'pwsh.exe' -and $CommandType -eq 'Application' }
        Should -Invoke Get-PowerShellVersion -Times 1 -Exactly -ParameterFilter { $Path -eq $path }
        Should -Invoke Invoke-WebRequest -Times 0 -Exactly
        Should -Invoke Expand-Archive -Times 0 -Exactly
        $script:events[-1] | Should -BeExactly "##vso[task.prependpath]$(Split-Path -Parent $path)"
    }

    It 'reuses a standard installation that is not on the agent PATH' {
        New-Item -Path (Split-Path -Parent $script:standard) -ItemType Directory -Force | Out-Null
        New-Item -Path $script:standard -ItemType File | Out-Null
        Mock Get-PowerShellVersion { '7.6.0' }

        Install-PowerShellCore -Version $script:version -Sha256 $script:sha256 -TempDirectory $script:temp

        Should -Invoke Invoke-WebRequest -Times 0 -Exactly
        Should -Invoke Get-PowerShellVersion -Times 1 -Exactly -ParameterFilter { $Path -eq $script:standard }
        $script:events[-1] | Should -BeExactly "##vso[task.prependpath]$(Split-Path -Parent $script:standard)"
    }

    It 'downloads and verifies the pinned ZIP, then probes it before exporting PATH' {
        Install-PowerShellCore -Version $script:version -Sha256 $script:sha256 -TempDirectory $script:temp

        Should -Invoke Invoke-WebRequest -Times 1 -Exactly -ParameterFilter {
            $Uri -eq 'https://github.com/PowerShell/PowerShell/releases/download/v7.4.0/PowerShell-7.4.0-win-x64.zip' -and $UseBasicParsing
        }
        Should -Invoke Get-FileHash -Times 1 -Exactly -ParameterFilter { $LiteralPath -eq $script:archive -and $Algorithm -eq 'SHA256' }
        Should -Invoke Expand-Archive -Times 1 -Exactly -ParameterFilter { $LiteralPath -eq $script:archive }
        Split-Path -Parent $script:archive | Should -BeExactly $script:temp
        $script:archive | Should -BeLike '*.zip'
        $script:probePath | Should -BeExactly (Join-Path $script:destination 'pwsh.exe')
        $script:events.IndexOf('download') | Should -BeLessThan $script:events.IndexOf('hash')
        $script:events.IndexOf('hash') | Should -BeLessThan $script:events.IndexOf('expand')
        $script:events.IndexOf('expand') | Should -BeLessThan $script:events.IndexOf('probe')
        $script:events[-1] | Should -BeExactly "##vso[task.prependpath]$($script:destination)"
        $script:events[-1] | Should -Not -Match '"'
        Should -Invoke Remove-Item -Times 1 -Exactly -ParameterFilter { $LiteralPath -eq $script:archive }
    }

    It 'uses a fresh directory rather than trusting a partial previous extraction' {
        Install-PowerShellCore -Version $script:version -Sha256 $script:sha256 -TempDirectory $script:temp
        $first = $script:destination
        Install-PowerShellCore -Version $script:version -Sha256 $script:sha256 -TempDirectory $script:temp

        $script:destination | Should -Not -Be $first
        Split-Path -Parent $script:destination | Should -BeExactly $script:temp
    }

    It 'fails before extraction if downloading fails' {
        Mock Invoke-WebRequest { throw 'download failed' }

        { Install-PowerShellCore -Version $script:version -Sha256 $script:sha256 -TempDirectory $script:temp } | Should -Throw '*download failed*'

        Should -Invoke Expand-Archive -Times 0 -Exactly
        Should -Invoke Get-PowerShellVersion -Times 0 -Exactly
        Should -Invoke Write-Host -Times 0 -Exactly -ParameterFilter { "$Object" -like '##vso*' }
    }

    It 'fails before extraction if the checksum does not match' {
        Mock Get-FileHash { [pscustomobject]@{ Hash = ('0' * 64) } }

        { Install-PowerShellCore -Version $script:version -Sha256 $script:sha256 -TempDirectory $script:temp } | Should -Throw '*SHA-256*'

        Should -Invoke Expand-Archive -Times 0 -Exactly
        Should -Invoke Write-Host -Times 0 -Exactly -ParameterFilter { "$Object" -like '##vso*' }
    }

    It 'does not export PATH if extraction fails' {
        Mock Expand-Archive { throw 'extraction failed' }

        { Install-PowerShellCore -Version $script:version -Sha256 $script:sha256 -TempDirectory $script:temp } | Should -Throw '*extraction failed*'

        Should -Invoke Get-PowerShellVersion -Times 0 -Exactly
        Should -Invoke Write-Host -Times 0 -Exactly -ParameterFilter { "$Object" -like '##vso*' }
    }

    It 'does not export PATH if the archive contains no executable at its root' {
        Mock Get-PowerShellVersion { & $originalProbe -Path $Path }

        { Install-PowerShellCore -Version $script:version -Sha256 $script:sha256 -TempDirectory $script:temp } | Should -Throw '*does not exist*'

        Should -Invoke Write-Host -Times 0 -Exactly -ParameterFilter { "$Object" -like '##vso*' }
    }

    It 'does not export PATH if the executable fails its probe' {
        Mock Get-PowerShellVersion { throw 'PowerShell exited with code 23' }

        { Install-PowerShellCore -Version $script:version -Sha256 $script:sha256 -TempDirectory $script:temp } | Should -Throw '*code 23*'

        Should -Invoke Write-Host -Times 0 -Exactly -ParameterFilter { "$Object" -like '##vso*' }
    }

    It 'does not export PATH if the downloaded executable reports a different version' {
        Mock Get-PowerShellVersion { '7.5.0' }

        { Install-PowerShellCore -Version $script:version -Sha256 $script:sha256 -TempDirectory $script:temp } | Should -Throw "*7.5.0*7.4.0*"

        Should -Invoke Write-Host -Times 0 -Exactly -ParameterFilter { "$Object" -like '##vso*' }
    }

    It 'rejects invalid settings before downloading: <Version>, <Sha256>, <TempDirectory>' -TestCases @(
        @{ Version = ''; Sha256 = ('0' * 64); TempDirectory = 'valid' }
        @{ Version = '../7.4.0'; Sha256 = ('0' * 64); TempDirectory = 'valid' }
        @{ Version = '7.4.0'; Sha256 = ''; TempDirectory = 'valid' }
        @{ Version = '7.4.0'; Sha256 = 'invalid'; TempDirectory = 'valid' }
        @{ Version = '7.4.0'; Sha256 = ('0' * 64); TempDirectory = '' }
    ) {
        param ($Version, $Sha256, $TempDirectory)
        if ($TempDirectory -eq 'valid') {
            $TempDirectory = $script:temp
        }

        { Install-PowerShellCore -Version $Version -Sha256 $Sha256 -TempDirectory $TempDirectory } | Should -Throw '*must*'

        Should -Invoke Invoke-WebRequest -Times 0 -Exactly
        Should -Invoke Write-Host -Times 0 -Exactly -ParameterFilter { "$Object" -like '##vso*' }
    }
}

Describe 'Get-PowerShellVersion with real native processes' {
    BeforeAll {
        . (Join-Path $PSScriptRoot 'install-powershell-core.ps1')
        $hostPowerShell = (Get-Process -Id $PID).Path
    }

    It 'waits for the executable to report its version and overwrites a stale exit code' {
        $global:LASTEXITCODE = 1

        Get-PowerShellVersion -Path $hostPowerShell | Should -BeExactly $PSVersionTable.PSVersion.ToString()

        $LASTEXITCODE | Should -Be 0
    }

    It 'fails if the executable is missing instead of trusting an old successful exit code' {
        $global:LASTEXITCODE = 0

        { Get-PowerShellVersion -Path (Join-Path $TestDrive 'missing pwsh.exe') } | Should -Throw '*does not exist*'
    }

    It 'rejects a native nonzero exit code' -Skip:($env:OS -eq 'Windows_NT') {
        $global:LASTEXITCODE = 0

        { Get-PowerShellVersion -Path '/usr/bin/false' } | Should -Throw '*code 1*'
    }

    It 'rejects a successful native process that does not report a version' -Skip:($env:OS -eq 'Windows_NT') {
        { Get-PowerShellVersion -Path '/usr/bin/true' } | Should -Throw '*version*'
    }

    It 'launches the exact executable from a directory containing spaces' -Skip:($env:OS -eq 'Windows_NT') {
        $directory = Join-Path $TestDrive 'PowerShell With Spaces'
        New-Item -Path $directory -ItemType Directory | Out-Null
        $executable = Join-Path $directory 'pwsh.exe'
        New-Item -Path $executable -ItemType SymbolicLink -Target $hostPowerShell | Out-Null

        Get-PowerShellVersion -Path $executable | Should -BeExactly $PSVersionTable.PSVersion.ToString()
    }

    It 'keeps <ScriptName> parseable and ASCII without a BOM' -TestCases @(
        @{ ScriptName = 'install-powershell-core.ps1' }
        @{ ScriptName = 'test-powershell-core-native.ps1' }
    ) {
        param ($ScriptName)
        $path = Join-Path $PSScriptRoot $ScriptName
        $bytes = [System.IO.File]::ReadAllBytes($path)

        @($bytes | Where-Object { $_ -gt 127 }) | Should -BeNullOrEmpty
        $tokens = $null
        $parseErrors = $null
        $null = [System.Management.Automation.Language.Parser]::ParseFile($path, [ref] $tokens, [ref] $parseErrors)
        $parseErrors | Should -BeNullOrEmpty
    }
}

Describe 'Native failures under Windows PowerShell 5.1' -Skip:($env:OS -ne 'Windows_NT') {
    BeforeAll {
        $powershell = Join-Path $env:SystemRoot 'System32/WindowsPowerShell/v1.0/powershell.exe'
        $harness = Join-Path $PSScriptRoot 'test-powershell-core-native.ps1'
        Test-Path -LiteralPath $powershell -PathType Leaf | Should -BeTrue
        $ErrorActionPreference = 'Continue'
        $global:LASTEXITCODE = $null
        $output = & $powershell -NoLogo -NoProfile -NonInteractive -ExecutionPolicy Bypass -File $harness -FixtureRoot $TestDrive 2>&1
        $exitCode = $LASTEXITCODE
        $exitCode | Should -Be 0 -Because ($output -join [Environment]::NewLine)
        $nativeResults = ($output -join [Environment]::NewLine) | ConvertFrom-Json
        @($nativeResults) | Should -HaveCount 2
    }

    It 'rejects a real nonzero native exit rather than a stale successful exit' {
        $result = @($nativeResults | Where-Object ExpectedExitCode -eq 1)
        $result | Should -HaveCount 1
        $result[0].PowerShellEdition | Should -BeExactly 'Desktop'
        ([version] $result[0].PowerShellVersion).Major | Should -Be 5
        ([version] $result[0].PowerShellVersion).Minor | Should -Be 1
        $result[0].NativeExitCode | Should -Be 1
        $result[0].Message | Should -BeExactly "PowerShell executable '$($result[0].Fixture)' exited with code 1."
    }

    It 'rejects a real successful native process with no version output' {
        $result = @($nativeResults | Where-Object ExpectedExitCode -eq 0)
        $result | Should -HaveCount 1
        $result[0].PowerShellEdition | Should -BeExactly 'Desktop'
        ([version] $result[0].PowerShellVersion).Major | Should -Be 5
        ([version] $result[0].PowerShellVersion).Minor | Should -Be 1
        $result[0].NativeExitCode | Should -Be 0
        $result[0].Message | Should -BeExactly "PowerShell executable '$($result[0].Fixture)' did not report a version."
    }
}

Describe 'Portable archive with real hashing and extraction' {
    BeforeAll {
        . (Join-Path $PSScriptRoot 'install-powershell-core.ps1')
        $savedProgramFiles = $env:ProgramFiles
    }

    BeforeEach {
        $script:messages = [System.Collections.Generic.List[string]]::new()
        $script:temp = Join-Path $TestDrive ([guid]::NewGuid().ToString('N') + ' Agent Temp')
        $script:fixture = Join-Path $TestDrive 'fixture.zip'
        New-Item -Path $script:temp -ItemType Directory | Out-Null
        $env:ProgramFiles = Join-Path $script:temp 'Missing Program Files'
        Mock Get-Command { $null } -ParameterFilter { $Name -eq 'pwsh.exe' }
        Mock Invoke-WebRequest { Copy-Item -LiteralPath $script:fixture -Destination $OutFile }
        Mock Get-PowerShellVersion {
            Test-Path -LiteralPath $Path -PathType Leaf | Should -BeTrue
            '7.4.0'
        }
        Mock Write-Host { $script:messages.Add(($Object -join ' ')) }
    }

    AfterAll {
        $env:ProgramFiles = $savedProgramFiles
    }

    It 'extracts the executable at the archive root and deletes only the downloaded ZIP' {
        $source = Join-Path $TestDrive 'pwsh.exe'
        Set-Content -LiteralPath $source -Value 'portable archive fixture' -NoNewline
        Compress-Archive -LiteralPath $source -DestinationPath $script:fixture -Force
        $sha256 = (Get-FileHash -LiteralPath $script:fixture -Algorithm SHA256).Hash

        Install-PowerShellCore -Version '7.4.0' -Sha256 $sha256 -TempDirectory $script:temp

        $directory = @(Get-ChildItem -LiteralPath $script:temp -Directory)
        $directory | Should -HaveCount 1
        Get-Content -Raw (Join-Path $directory[0].FullName 'pwsh.exe') | Should -BeExactly 'portable archive fixture'
        @(Get-ChildItem -LiteralPath $script:temp -Filter '*.zip') | Should -HaveCount 0
        Test-Path -LiteralPath $script:fixture | Should -BeTrue
        $script:messages[-1] | Should -BeExactly "##vso[task.prependpath]$($directory[0].FullName)"
    }

    It 'fails on a corrupt ZIP even when its download checksum matches' {
        Set-Content -LiteralPath $script:fixture -Value 'not a ZIP archive'
        $sha256 = (Get-FileHash -LiteralPath $script:fixture -Algorithm SHA256).Hash

        { Install-PowerShellCore -Version '7.4.0' -Sha256 $sha256 -TempDirectory $script:temp } | Should -Throw

        Should -Invoke Get-PowerShellVersion -Times 0 -Exactly
        Should -Invoke Write-Host -Times 0 -Exactly -ParameterFilter { "$Object" -like '##vso*' }
    }
}

Describe 'PowerShell bootstrap process exit status' {
    BeforeAll {
        $hostPowerShell = (Get-Process -Id $PID).Path
        $bootstrap = Join-Path $PSScriptRoot 'install-powershell-core.ps1'
    }

    BeforeEach {
        $savedPath = $env:PATH
        $savedProgramFiles = $env:ProgramFiles
    }

    AfterEach {
        $env:PATH = $savedPath
        $env:ProgramFiles = $savedProgramFiles
    }

    It 'returns a nonzero process exit code and never advertises PATH on failure' {
        $ErrorActionPreference = 'Continue'
        $env:PATH = $TestDrive
        $env:ProgramFiles = Join-Path $TestDrive 'Missing Program Files'

        $output = & $hostPowerShell -NoLogo -NoProfile -NonInteractive -File $bootstrap -Version 'invalid' -Sha256 'invalid' -TempDirectory 'invalid' 2>&1
        $exitCode = $LASTEXITCODE

        $exitCode | Should -Not -Be 0
        "$output" | Should -Match 'POWERSHELL_VERSION must'
        "$output" | Should -Not -Match '##vso'
    }

    It 'returns success after a real probe when reusing an application on PATH' -Skip:($env:OS -eq 'Windows_NT') {
        $directory = Join-Path $TestDrive 'Existing PowerShell'
        New-Item -Path $directory -ItemType Directory | Out-Null
        New-Item -Path (Join-Path $directory 'pwsh.exe') -ItemType SymbolicLink -Target $hostPowerShell | Out-Null
        $env:PATH = $directory

        $output = & $hostPowerShell -NoLogo -NoProfile -NonInteractive -File $bootstrap -Version 'invalid' -Sha256 'invalid' -TempDirectory 'invalid' 2>&1
        $exitCode = $LASTEXITCODE

        $exitCode | Should -Be 0
        "$output" | Should -Match ([regex]::Escape("##vso[task.prependpath]$directory"))
    }
}
