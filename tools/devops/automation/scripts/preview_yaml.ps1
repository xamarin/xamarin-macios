param (
    [Parameter(Mandatory)]
    [String]
    $AccessToken,

    [String]
    $Pipeline="ci",

    [String]
    $Branch="main",

    [Parameter(Mandatory)]
    [String]
    $OutputFile
)

Import-Module ./VSTS.psm1 -Force

if ($Pipeline -eq "pr") {
    Write-Host "Querying the PR pipeline."
    $PipelineId="16533"
} else {
    Write-Host "Querying the CI pipeline."
    $PipelineId="14411"
}

Get-YamlPreview -Org "devdiv" -Project "DevDiv" -AccessToken $AccessToken -PipelineId $PipelineId -Branch $Branch -OutputFile $OutputFile
