param
(
    [Parameter(Mandatory)]
    [String]
    $SourceBranch,

    [String]
    $IsPr
)

if($IsPr.ToLower() -eq "true") {
    Write-Host "Working on a PR, Undoing the github merge with main."

    $refspec="+refs/pull/$Env:SYSTEM_PULLREQUEST_PULLREQUESTNUMBER/*:refs/remotes/origin/pull/$Env:SYSTEM_PULLREQUEST_PULLREQUESTNUMBER/*"

    Write-Host "Refspec: $refspec"

    git fetch origin --no-tags "$refspec"

    $branch="$SourceBranch".Replace("merge", "head")
    $branch=$branch.Replace("refs", "origin")

    Write-Host "Checking out branch $branch"

    git checkout $branch
    git submodule update --recursive --init
    git clean -xffd
    git submodule foreach --recursive git clean -xffd
    $hash = git rev-parse HEAD

    Write-Host "##vso[task.setvariable variable=GIT_HASH;isOutput=true]$hash"
} else {
    Write-Host "Working on a branch, using current commit"
    $hash = git rev-parse HEAD
    Write-Host "##vso[task.setvariable variable=GIT_HASH;isOutput=true]$hash"
}
