param (
    [Parameter(Mandatory)]
    [String]
    $XamarinTop,

    [Parameter(Mandatory)]
    [String]
    $BuildReason
)

# we have a number of scripts that require to be executed from the top of the src, rather
# than keeping track of the location of the script, we create two env vars that can be used to
# get to the top
$configVars = @{} # var name/value pair, later print twice, to process and debug

Write-Host "##vso[task.setvariable variable=XAM_TOP]$XamarinTop"
Write-Host "PrID in env is $Env:PR_ID"

if (-not [string]::IsNullOrEmpty($Env:PR_ID)) {
    $configVars.Add("PR_ID", $Env:PR_ID)
} else {
    $configVars.Add("PR_ID", "")
}

# decide if we are dealing with a PR or a re-triggered PR or a build from
# a branch in origin

if ($BuildReason -eq "PullRequest" -or (($BuildReason -eq "Manual" -or $BuildReason -eq "IndividualCI") -and ($BuildSourceBranchName -eq "merge")) ) {
    $configVars.Add("BuildPkgs", "True")

    # interesting case, we have build-pkg and skip-pkg... if that is the case, we build it, but we set a warning
    if ($Env:BuildPackage -eq "True" -and $Env:SkipPackages -eq "True") {
        Write-Host "##vso[task.logissue type=warning]'build-package' and 'skip-packages' are both present. Building packages in case of a doubt."
        $configVars.Add("BuildPkgs", "True")
    }

    if ($Env:SkipNugets -eq "True") {
        $configVars.Add("BuildNugets", "False")
    } else {
        $configVars.Add("BuildNugets", "True")
    }

    if ($Env:SkipSigning -eq "True") {
        $configVars.Add("SignPkgs", "False")
    } else {
        $configVars.Add("SignPkgs", "True")
    }

    $configVars.Add("RunSampleTests", $Env:RunSampleTests)

} else {
    # set the defaults, all the things! o/
    # build pkg, nugets and sign them
    $configVars.Add("BuildPkgs", "True")
    $configVars.Add("BuildNugets", "True")
    $configVars.Add("SignPkgs", "True")

    # tests, run all of them, internal, external, mac but not sample tests
    $configVars.Add("RunSampleTests", "False")
}
# write debugging and process of the vars
foreach($key in $configVars.Keys) {
    Write-Host "$key='$($configVars[$key])'"
    Write-Host "##vso[task.setvariable variable=$key;isOutput=true]$($configVars[$key])"
}
