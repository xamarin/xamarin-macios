 <#
    .SYNOPSIS
        Peform a dry run of the xamarin-macios pipelines that will
        download the expanded yaml.
    .DESCRIPTION
        This script helps tp debug issues that happen in the azure devops 
        pipelines by performing a dry run of the yaml and returning its expanded
        version. That way a developer can see the full yaml and find issues without running
        the pipelines.
    .EXAMPLE

        Query the expanded template from the ci pipeline in main:

        ./preview_yaml.ps1 -AccessToken $pat -OutputFile ./full.yaml

        Query the expansion in the pr pipeline:

        ./preview_yaml.ps1 -AccessToken $pat -OutputFile ./full.yaml -Pipeline "pr"

        Query the expansion in the ci pipeline for a diff branch than main:

        ./preview_yaml.ps1 -AccessToken $pat -OutputFile ./full.yaml -Branch "trigger-issues"

    .PARAMETER AccessToken
        Personal access token for VSTS that has at least access to the pipelines API.

    .PARAMETER Pipeline
        Name of the pipeline to use for the dry run, it accepts two values:
            - ci
            - pr
        The defualt value is ci.
    .PARAMETER Branch
        The branch to be used for the dry run. Takes the name of the branch, it should
        not have the prefix refs/head.

    .PARAMETER OutputFile
        Path to the file where the expanded yaml will be written. The output file is 
        ALLWAYS overwritten.

#>
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
