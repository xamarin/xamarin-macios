<#
    .SYNOPSIS
        Returns a hash table with the all the installed versions of a framework and the current version selected.
#>
function Get-FrameworkVersions {
    [CmdletBinding()]
    [OutputType('Hashtable')]
    param
    (
        [Parameter(Mandatory)]
        [String]
        [ValidateScript({
            Test-Path -Path $_ -PathType Container  # framework path should be a directory and exist
        })]
        $Path
    )

    $versionsPath = [System.IO.Path]::Combine($Path, "Versions")
    Write-Debug "Searching for version in $versionsPath"

    if ( -not (Test-Path $versionsPath -PathType Container)) {
        Write-Debug "Path '$versionsPath' was not found."
        return @{}
    }

    $versionsInformation = [Ordered] @{
        Versions = Get-ChildItem $versionsPath -Exclude "Current" -Name # exclude current for this line
    }

    # get the current link and the path it points to
    $currentPath = [System.IO.Path]::Combine($versionsPath, "Current")
    $currentVersion = Get-Item -Path $currentPath
    $versionsInformation["Current"] = $currentVersion.Target
    return $versionsInformation
}

<#
    .SYNOPSIS
        Returns the version of Xcode selected via xcode-select
#>
function Get-SelectedXcode {

    [CmdletBinding()]
    [OutputType('String')]
    param()

    # powershell does not have a nice way to execute a process and read the stdout, we use .net
    $pinfo = New-Object System.Diagnostics.ProcessStartInfo
    $pinfo.FileName = "xcode-select"
    $pinfo.RedirectStandardOutput = $true
    $pinfo.UseShellExecute = $false
    $pinfo.Arguments = "-p"

    $p = New-Object System.Diagnostics.Process
    $p.StartInfo = $pinfo
    $p.Start() | Out-Null
    $path = $p.StandardOutput.ReadToEnd().Trim().Replace("/Contents/Developer", "")
    $p.WaitForExit()
    return $path
}

<#
    .SYNOPSIS
        Returns the current mono version.
#>
function Get-MonoVersion {
    [CmdletBinding()]
    [OutputType('String')]
    param()

    # powershell does not have a nice way to execute a procss and read the stdout, we use .net
    $pinfo = New-Object System.Diagnostics.ProcessStartInfo
    $pinfo.FileName = "mono"
    $pinfo.RedirectStandardOutput = $true
    $pinfo.UseShellExecute = $false
    $pinfo.Arguments = "--version"

    $p = New-Object System.Diagnostics.Process
    $p.StartInfo = $pinfo
    $p.Start() | Out-Null
    $rv = $p.StandardOutput.ReadToEnd().Trim()
    $p.WaitForExit()
    return $rv
}

<#
    .SYNOPSIS
        Removes all the installed simulators in the system.
#>
function Remove-InstalledSimulators {
    param()
    # use the .Net libs to execute the process
    $pinfo = New-Object System.Diagnostics.ProcessStartInfo
    $pinfo.FileName = "/Applications/Xcode.app/Contents/Developer/usr/bin/simctl"
    $pinfo.RedirectStandardOutput = $true
    $pinfo.UseShellExecute = $false
    $pinfo.Arguments = "delete all"

    $p = New-Object System.Diagnostics.Process
    $p.StartInfo = $pinfo
    $p.Start() | Out-Null
    $p.WaitForExit()
}

<#
    .SYNOPSIS
        Returns the details of the system that is currently executing the
        pipeline.
    .DESCRIPTION
        This function returns the following details of the system that is
        being used to execute the pipeline. Those details include:

          * Runtime info
          * OS information
          * Xamarin.iOS installed versions
          * Xamarin.Mac installed versions
          * Xcode installed applications
          * Xcode current selected version
          * Mono version
          * Uptime
          * Free HD space
          * Used HD space
#>
function Get-SystemInfo {

    [CmdletBinding()]
    [OutputType('Hashtable')]
    [CmdletBinding()]
    param ()
    if ($IsMacOS) {
        $drive = Get-PSDrive "/"
    } else {
        $drive = Get-PSDrive "C"
    }
    # created and ordered dictionary with the data
    $systemInfo = [Ordered]@{
        OSDescription = [System.Runtime.InteropServices.RuntimeInformation]::OSDescription;
        OSArchitecture = [System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture;
        Runtime = [System.Runtime.InteropServices.RuntimeInformation]::FrameworkDescription;
        Uptime = Get-Uptime
        FreeStorage = "$($drive.Free / 1GB) GB";
        UsedStorage = "$($drive.Used / 1GB) GB";
    }

    if ($IsMacOS) {
        $xamariniOSVersions = Get-FrameworkVersions -Path "/Library/Frameworks/Xamarin.iOS.framework"
        $xamarinMacVersions = Get-FrameworkVersions -Path "/Library/Frameworks/Xamarin.Mac.framework"

        $systemInfo["XamariniOSVersions"] = $xamariniOSVersions.Versions
        $systemInfo["XamariniOSCurrentVersion"] = $xamariniOSVersions.Current
        $systemInfo["XamarinMacVersions"] = $xamarinMacVersions.Versions
        $systemInfo["XamarinMacCurrentVersion"] = $xamarinMacVersions.Current
        $systemInfo["XcodeVersions"] = Get-ChildItem "/Applications" -Include "Xcode*" -Name
        $systemInfo["XcodeSelected"] = Get-SelectedXcode
        $systemInfo["MonoVersion"] = Get-MonoVersion
    }

    return $systemInfo
}

<#
    .SYNOPSIS
        Remove known processes from other runs.
    .DESCRIPTION
        Remove all known processes to xamarin that might have been left
        behind after other runs.
#>
function Clear-XamarinProcesses {
    # could be cleaner or smarter, but is not large atm
    Start-Process -FilePath "pkill" -ArgumentList "-9 mlaunch" -NoNewWindow -PassThru -Wait
    Write-Debug "mlaunch terminated"
    Start-Process -FilePath "pkill" -ArgumentList "-9 -f mono.*xharness.exe" -NoNewWindow -PassThru -Wait
    Write-Debug "xharness terminated"
    Start-Process -FilePath "pkill" -ArgumentList "-9 -f ssh.*rsync.*xamarin-storage" -NoNewWindow -PassThru -Wait
    Write-Debug "rsync terminater"
}

<#
    .SYNOPSIS
       Clear all possible leftovers after the tests. 
#>
function Clear-AfterTests {
    Get-PSDrive "/" | Format-Table -Wrap

    # common dirs to delete
    $directories = @(
        "/Applications/Visual\ Studio*",
        "~/Library/Caches/VisualStudio",
        "~/Library/Logs/VisualStudio",
        "~/Library/VisualStudio",
        "~/Library/Preferences/Xamarin",
        "~/Library/Caches/com.xamarin.provisionator"
    )

    foreach ($dir in $directories) {
        Write-Debug "Removing $dir"
        try {
            if (Test-Path -Path $dir) {
                Remove-Item –Path $dir -Recurse -ErrorAction SilentlyContinue -Force
            } else {
                Write-Debug "Path not found '$dir'"
            }
        } catch {
            Write-Error "Could not remove dir $dir - $_"
        }
    }
    Get-PSDrive "/" | Format-Table -Wrap
}

<#
    .SYNOPSIS
        Checks if there is enough space in the HD
#>
function Test-HDFreeSpace {
    param
    (
        [Parameter(Mandatory)]
        [int]
        $Size
    )
    $drive = Get-PSDrive "/"
    return $drive.Free / 1GB -gt $Size
}

# module exports, any other functions are private and should not be used outside the module
Export-ModuleMember -Function Get-SystemInfo
Export-ModuleMember -Function Clear-XamarinProcesses 
Export-ModuleMember -Function Test-HDFreeSpace
Export-ModuleMember -Function Clear-AfterTests
Export-ModuleMember -Function Remove-InstalledSimulators 
