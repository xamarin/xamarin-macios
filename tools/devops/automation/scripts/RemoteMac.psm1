<#
    .SYNOPSIS
        Executes a command on a remote machine using scp
#>

function Invoke-SshCommand {
    param (
        [Parameter(Mandatory)]
        [string]
        $RemoteHost,

        [Parameter(Mandatory)]
        [string]
        $RemoteUserName,

        [Parameter(Mandatory, ValueFromRemainingArguments)]
        [string[]]
        $CommandArguments
    )

    $cmd = "ssh -v -i `"$Env:ID_RSA_PATH`" -o StrictHostKeyChecking=no `"$($RemoteUserName)@$($RemoteHost)`" $CommandArguments"
    Write-Host "Executing: $cmd"

    ssh -v -i "$Env:ID_RSA_PATH" -o StrictHostKeyChecking=no "$($RemoteUserName)@$($RemoteHost)" @CommandArguments

    if ($LastExitCode -ne 0) {
        throw [System.Exception]::new("Failed to execute ssh command '$cmd', exit code: $LastExitCode")
    }
}

<#
    .SYNOPSIS
        Uploads a file or directory to a remote machine using scp
#>
function Invoke-SshDownload {
    param (
        [Parameter(Mandatory)]
        [string]
        $RemoteHost,

        [Parameter(Mandatory)]
        [string]
        $RemoteUserName,

        [Parameter(Mandatory)]
        [string]
        $Source,

        [Parameter(Mandatory)]
        [string]
        $Target
    )

    $cmd = "scp -v -i $Env:ID_RSA_PATH -o StrictHostKeyChecking=no $($RemoteUserName)@$($RemoteHost):$Source $Target"
    Write-Host "Executing: $cmd"

    scp -v -i "$Env:ID_RSA_PATH" -o StrictHostKeyChecking=no "$($RemoteUserName)@$($RemoteHost):$Source" $Target

    if ($LastExitCode -ne 0) {
        throw [System.Exception]::new("Failed to execute scp command '$cmd', exit code: $LastExitCode")
    }
}

Export-ModuleMember -Function Invoke-SshCommand
Export-ModuleMember -Function Invoke-SshDownload
