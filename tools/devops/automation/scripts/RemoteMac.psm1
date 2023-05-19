<#
    .SYNOPSIS
        Executes a command on a remote machine using sshenv
#>

function Invoke-SshEnvCommand {
    param (
        [Parameter(Mandatory)]
        [string]
        $SourcesDirectory,

        [Parameter(Mandatory)]
        [string]
        $DotNet,

        [Parameter(Mandatory)]
        [string]
        $RemoteHost,

        [Parameter(Mandatory)]
        [string]
        $RemoteUserName,

        [Parameter(Mandatory)]
        [string]
        $RemotePasswordEnvironmentVariable,

        [Parameter(Mandatory)]
        [bool]
        $ThrowIfError,

        [Parameter(Mandatory, ValueFromRemainingArguments)]
        [string[]]
        $CommandArguments
    )

    $cmd = [System.String]::Join(" ", $CommandArguments)

    Write-Host "Command is: $cmd"
    Write-Host "There are $($CommandArguments.Length) arguments in the command."

    & $DotNet `
      run `
      --project "$SourcesDirectory\xamarin-macios\tools\sshenv\sshenv.csproj" `
      -- `
      --host $RemoteHost `
      --user $RemoteUserName `
      --penv $RemotePasswordEnvironmentVariable `
      @CommandArguments

    if ($ThrowIfError) {
        if ($LastExitCode -ne 0) {
            throw [System.Exception]::new("Failed to execute sshenv command, exit code: $LastExitCode")
        }
    }
    Write-Host "sshenv command returned exit code: $LastExitCode"
}

<#
    .SYNOPSIS
        Uploads a file or directory to a remote machine using sshenv
#>
function Invoke-SshEnvUpload {
    param (
        [Parameter(Mandatory)]
        [string]
        $SourcesDirectory,

        [Parameter(Mandatory)]
        [string]
        $DotNet,

        [Parameter(Mandatory)]
        [string]
        $RemoteHost,

        [Parameter(Mandatory)]
        [string]
        $RemoteUserName,

        [Parameter(Mandatory)]
        [string]
        $RemotePasswordEnvironmentVariable,

        [Parameter(Mandatory)]
        [bool]
        $ThrowIfError,

        [Parameter(Mandatory)]
        [string]
        $Source,

        [Parameter(Mandatory)]
        [string]
        $Target
    )

    & $DotNet `
      run `
      --project "$SourcesDirectory\xamarin-macios\tools\sshenv\sshenv.csproj" `
      -- `
      --host $RemoteHost `
      --user $RemoteUserName `
      --penv $RemotePasswordEnvironmentVariable `
      --mode upload `
      --source $Source `
      --target $Target

    if ($ThrowIfError) {
        if ($LastExitCode -ne 0) {
            throw [System.Exception]::new("Failed to execute sshenv command, exit code: $LastExitCode")
        }
    }
    Write-Host "sshenv command returned exit code: $LastExitCode"
}
<#
    .SYNOPSIS
        Uploads a file or directory to a remote machine using sshenv
#>
function Invoke-SshEnvDownload {
    param (
        [Parameter(Mandatory)]
        [string]
        $SourcesDirectory,

        [Parameter(Mandatory)]
        [string]
        $DotNet,

        [Parameter(Mandatory)]
        [string]
        $RemoteHost,

        [Parameter(Mandatory)]
        [string]
        $RemoteUserName,

        [Parameter(Mandatory)]
        [string]
        $RemotePasswordEnvironmentVariable,

        [Parameter(Mandatory)]
        [bool]
        $ThrowIfError,

        [Parameter(Mandatory)]
        [string]
        $Source,

        [Parameter(Mandatory)]
        [string]
        $Target
    )

    & $DotNet `
      run `
      --project "$SourcesDirectory\xamarin-macios\tools\sshenv\sshenv.csproj" `
      -- `
      --host $RemoteHost `
      --user $RemoteUserName `
      --penv $RemotePasswordEnvironmentVariable `
      --mode download `
      --source $Source `
      --target $Target

    if ($ThrowIfError) {
        if ($LastExitCode -ne 0) {
            throw [System.Exception]::new("Failed to execute sshenv command, exit code: $LastExitCode")
        }
    }
    Write-Host "sshenv command returned exit code: $LastExitCode"
}

<#
    .SYNOPSIS
        Installs .NET on a remote Mac, together with our workloads.
#>
function Install-DotNetOnRemoteMac {
    param (
        [Parameter(Mandatory)]
        [string]
        $SourcesDirectory,

        [Parameter(Mandatory)]
        [string]
        $DotNet,

        [Parameter(Mandatory)]
        [string]
        $UploadDirectory,

        [Parameter(Mandatory)]
        [string]
        $RemoteHost,

        [Parameter(Mandatory)]
        [string]
        $RemoteUserName,

        [Parameter(Mandatory)]
        [string]
        $RemotePasswordEnvironmentVariable
    )

    $SharedArguments = @{
        SourcesDirectory = $SourcesDirectory
        DotNet = $DotNet
        RemoteHost = $RemoteHost
        RemoteUserName = $RemoteUserName
        RemotePasswordEnvironmentVariable = $RemotePasswordEnvironmentVariable
    }

    Invoke-SshEnvCommand @SharedArguments -ThrowIfError $false ls -la "/Users/$RemoteUserName"
    Invoke-SshEnvCommand @SharedArguments -ThrowIfError $false rm -rf "/Users/$RemoteUserName/remote_build_testing"
    Invoke-SshEnvCommand @SharedArguments -ThrowIfError $false ls -la "/Users/$RemoteUserName"

    Invoke-SshEnvUpload  @SharedArguments -ThrowIfError $true  -Source $UploadDirectory -Target "/Users/$RemoteUserName/remote_build_testing"

    Invoke-SshEnvCommand @SharedArguments -ThrowIfError $false ls -la "/Users/$RemoteUserName/remote_build_testing"
    Invoke-SshEnvCommand @SharedArguments -ThrowIfError $false cat /Users/$RemoteUserName/remote_build_testing/configuration.json
    Invoke-SshEnvCommand @SharedArguments -ThrowIfError $true  chmod +x /Users/$RemoteUserName/remote_build_testing/install-on-mac.sh
    Invoke-SshEnvCommand @SharedArguments -ThrowIfError $false cat /Users/$RemoteUserName/remote_build_testing/install-on-mac.sh
    #Invoke-SshEnvCommand @SharedArguments -ThrowIfError $false hexdump -C /Users/$RemoteUserName/remote_build_testing/install-on-mac.sh
    Invoke-SshEnvCommand @SharedArguments -ThrowIfError $true  /bin/bash -eux -- /Users/$RemoteUserName/remote_build_testing/install-on-mac.sh
    Invoke-SshEnvCommand @SharedArguments -ThrowIfError $false ls -la "/Users/$RemoteUserName"
    Invoke-SshEnvCommand @SharedArguments -ThrowIfError $false ls -la "/Users/$RemoteUserName/remote_build_testing"
}

<#
    .SYNOPSIS
        Creates a directory of files needed to install workloads on a remote mac.
#>
function New-RemoteMacInstallDirectory {
    param (
        [Parameter(Mandatory)]
        [string]
        $SourcesDirectory,

        [Parameter(Mandatory)]
        [string]
        $ArtifactsDirectory
    )

    $uploadDirectory = "$ArtifactsDirectory/remote-mac-upload"
    New-Item -Path $uploadDirectory -ItemType "directory"
    New-Item -Path "$uploadDirectory/nupkg" -ItemType "directory"

    Copy-Item -Path "$SourcesDirectory/xamarin-macios/NuGet.config" -Destination "$uploadDirectory/NuGet.config"
    Copy-Item -Path "$SourcesDirectory/xamarin-macios/global.json" -Destination "$uploadDirectory/global.json"
    Copy-Item -Path "$SourcesDirectory/xamarin-macios/tests/dotnet/Windows/install-on-mac.sh" -Destination "$uploadDirectory/install-on-mac.sh"
    Copy-Item -Path "$ArtifactsDirectory/WorkloadRollback/WorkloadRollback.json" -Destination "$uploadDirectory/WorkloadRollback.json"
    Copy-Item -Path "$ArtifactsDirectory/build-configuration/configuration.json" -Destination "$uploadDirectory/configuration.json"
    Copy-Item -Path "$ArtifactsDirectory/not-signed-package/*.nupkg" -Destination "$uploadDirectory/nupkg"

    # Get-ChildItem -Path $uploadDirectory | Write-Host

    return $uploadDirectory
}

Export-ModuleMember -Function New-RemoteMacInstallDirectory
Export-ModuleMember -Function Install-DotNetOnRemoteMac
Export-ModuleMember -Function Invoke-SshEnvCommand
Export-ModuleMember -Function Invoke-SshEnvUpload
Export-ModuleMember -Function Invoke-SshEnvDownload
