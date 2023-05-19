<#
System scripts unit tests. 
#>

Import-Module ./RemoteMac.psm1 -Force

Describe 'New-RemoteMacInstallDirectory' {
    Context 'default' {
        It 'creates the correct contents' {

            $sourcesDirectory = [System.Environment]::CurrentDirectory

            while (-Not (Test-Path -Path "$sourcesDirectory/.git")) {
                $sourcesDirectory = [System.IO.Path]::GetDirectoryName($sourcesDirectory)
            }
            $sourcesDirectory = [System.IO.Path]::GetDirectoryName($sourcesDirectory)

            $artifactsDirectory = Join-Path "." "test-artifacts-dir"

            New-Item -Path "$artifactsDirectory" -ItemType "directory" -Force
            New-Item -Path "$artifactsDirectory/tmp/build-configuration" -ItemType "directory" -Force
            New-Item -Path "$artifactsDirectory/tmp/package-internal/WorkloadRollback" -ItemType "directory" -Force
            New-Item -Path "$artifactsDirectory/tmp/not-signed-package" -ItemType "directory" -Force

            Set-Content -Path "$artifactsDirectory/tmp/build-configuration/configuration.json" -Value "build-configuration/configuration.json"
            Set-Content -Path "$artifactsDirectory/tmp/package-internal/WorkloadRollback/WorkloadRollback.json" -Value "WorkloadRollback.json"
            Set-Content -Path "$artifactsDirectory/tmp/not-signed-package/test.nupkg" -Value "test.nupkg"

            $uploadDirectory = New-RemoteMacInstallDirectory -SourcesDirectory $sourcesDirectory -ArtifactsDirectory $artifactsDirectory

            $fileListing = Get-ChildItem -Path $uploadDirectory -Recurse | Out-String
            Write-Host "a"
            Write-Host $fileListing
            Write-Host "b"

            Remove-Item -Path $artifactsDirectory -Recurse

            # FIXME # $fileListing | Should -Be "?"
        }
    }
}

Describe 'Invoke-SshEnvCommand' {
    Context 'default' {
        It 'tries to do the right thing' {
            $sourcesDirectory = [System.Environment]::CurrentDirectory
            while (-Not (Test-Path -Path "$sourcesDirectory/.git")) {
                $sourcesDirectory = [System.IO.Path]::GetDirectoryName($sourcesDirectory)
            }
            $sourcesDirectory = [System.IO.Path]::GetDirectoryName($sourcesDirectory)

            $SharedArguments = @{
                SourcesDirectory = $SourcesDirectory
                DotNet = "donut"
                RemoteHost = "127.0.0.1"
                RemoteUserName = "$Env:USER"
                RemotePasswordEnvironmentVariable = "USER"
            }

            Invoke-SshEnvCommand @SharedArguments ls -la
        }
    }
}
