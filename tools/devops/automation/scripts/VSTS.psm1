class Agent {
    [object] $RestObject

    Agent (
        [object] $object) {
        $this.RestObject = $object
    }

    [string] GetID() {
        return $this.RestObject.id
    }

    [string] GetName() {
        return $this.RestObject.name
    }

    [bool] IsEnabled() {
        return $this.RestObject.enabled
    }
}


class Agents {
    [string] $Org
    hidden [string] $Token

    Agents(
        [string] $org,
        [string] $token) {
        $this.Org = $org
        $this.Token = $token
    }

    [object] GetAgent($pool, $name) {
        if (-not $pool) {
            throw [System.ArgumentNullException]::new("pool")
        }
        if (-not $name) {
            throw [System.ArgumentNullException]::new("name")
        }
        $url = "https://dev.azure.com/$($this.Org)/_apis/distributedtask/pools/$($pool.GetID())/agents?agentName=$name&includeCapabilities=true&api-version=6.0"
        $headers = Get-AuthHeader($this.Token)
        $agents = Invoke-RestMethod -Uri $url -Headers $headers -Method "GET"  -ContentType 'application/json'
        if ($agents.count -ne 1) {
            return $null
        }
        return [Agent]::new($agents.value[0])
    }

    [object[]] GetAgents($pool) {
        if (-not $pool) {
            throw [System.ArgumentNullException]::new("pool")
        }
        $url = "https://dev.azure.com/$($this.Org)/_apis/distributedtask/pools/$($pool.GetID())/agents?includeCapabilities=true&api-version=6.0"
        $headers = Get-AuthHeader($this.Token)
        $agents = Invoke-RestMethod -Uri $url -Headers $headers -Method "GET"  -ContentType 'application/json'
        $result = [System.Collections.ArrayList]@()
        foreach ($a in $agents.value) {
            $result.Add([Agent]::new($a))
        }
        return $result
    }

    [void] SetEnabled($pool, $agent, $isEnabled) {
        if (-not $pool) {
            throw [System.ArgumentNullException]::new("pool")
        }
        if (-not $agent) {
            throw [System.ArgumentNullException]::new("agent")
        }
        $url = "https://dev.azure.com/$($this.Org)/_apis/distributedtask/pools/$($pool.GetID())/agents/$($agent.GetID())?api-version=6.0"
        Write-Debug "Url is $url"
        $headers = Get-AuthHeader($this.Token)
        $payload = @{
            id = $agent.GetID() ;
            enabled = $isEnabled ;
        }
        Invoke-RestMethod -Uri $url -Headers $headers -Method "PATCH" -Body ($payload | ConvertTo-json) -ContentType 'application/json'
    }
}

class Pool {
    hidden [object] $RestObject

    Pool (
        [object] $data) {
        $this.RestObject = $data
    }

    [string] GetID() {
        return $this.RestObject.id
    }

    [string] GetName() {
        return $this.RestObject.name
    }

    [int] GetSize() {
        return $this.RestObject.size
    }
}

class Pools {

    [string] $Org
    hidden [string] $Token

    Pools(
        [string] $org,
        [string] $token) {
        $this.Org = $org
        $this.Token = $token
    }

    [object] GetPool ($name) {
        if (-not $name) {
            throw [System.ArgumentNullException]::new("name")
        }
        $url = "https://dev.azure.com/$($this.Org)/_apis/distributedtask/pools?poolName=$name&api-version=6.0"
        $headers = Get-AuthHeader($this.Token)
        $pools = Invoke-RestMethod -Uri $url -Headers $headers -Method "GET"  -ContentType 'application/json'
        if ($pools.count -ne 1) {
            return $null
        }
        return [Pool]::new($pools.value[0])
    }

    [object[]] GetPools () {
        $url = "https://dev.azure.com/$($this.Org)/_apis/distributedtask/pools?api-version=6.0"
        $headers = Get-AuthHeader($this.Token)
        $pools = Invoke-RestMethod -Uri $url -Headers $headers -Method "GET"  -ContentType 'application/json'

        # loop and create a pool object for each of the pools in the org
        $result = [System.Collections.ArrayList]@()
        foreach ($p in $pools.value) {
            $result.Add([Pool]::new($p))
        }
        return $result
    }
}

class Artifact {
    hidden [object] $RestObject

    Artifact (
        [object] $data) {
        $this.RestObject = $data
    }

    [string] GetDownloadTicket() {
        return $this.RestObject.resource.downloadTicket
    }

    [string] GetDownloadUrl() {
        return $this.RestObject.resource.downloadUrl
    }

    [string] GetName() {
        return $this.RestObject.name
    }

}

class Artifacts {
    [string] $Org
    [string] $Project
    hidden [string] $Token

    Artifacts (
        [string] $org,
        [string] $project,
        [string] $token) {
        $this.Org = $org
        $this.Project = $project
        $this.Token = $token
    }

    [object[]] GetArtifacts($buildId) {
        if (-not $buildId) {
            throw [System.ArgumentNullException]::new("buildId")
        }

        $url = "https://dev.azure.com/$($this.Org)/$($this.Project)/_apis/build/builds/$buildId/artifacts?api-version=6.0"
        $headers = Get-AuthHeader($this.Token)
        $artifacts = Invoke-RestMethod -Uri $url -Headers $headers -Method "GET"  -ContentType 'application/json'

        # loop and create a pool object for each of the pools in the org
        $result = [System.Collections.ArrayList]@()
        foreach ($a in $artifacts.value) {
            $result.Add([Artifact]::new($a))
        }
        return $result
    }

    [string] DownloadArtifact($artifact, $outputDir) {
        if (-not $artifact) {
            throw [System.ArgumentNullException]::new("artifact")
        }

        if (-not $outputDir) {
            throw [System.ArgumentNullException]::new("outputDir")
        }

        if (Test-Path -Path $outputDir -PathType Leaf) {
            throw [System.ArgumentException]::new("outputDir must be a directory. File found.")
        }

        $outputPath = Resolve-Path $outputDir
        $outputPath = Join-Path  $outputPath  -ChildPath "$($artifact.GetName()).zip"
        $headers = Get-AuthHeader($this.Token)
        $auth = $headers["Authorization"]
        # why using crul, well because with large files you will get an exception fron the internal methods from pwsh. Someone did not
        # expect me to download large files with Invoke-Rest
        curl $artifact.GetDownloadUrl() -L -o $outputPath --header "Authorization: $auth" 
        return $outputPath 
    }

}

class Vsts {
    [string] $Org
    [string] $Project
    hidden [string] $Token

    [Pools] $Pools
    [Agents] $Agents
    [Artifacts] $Artifacts

    VSTS (
        [string] $org,
        [string] $project,
        [string] $token) {
        $this.Org = $org
        $this.Project = $project
        $this.Token = $token
        # generate the helper objects
        $this.Pools = [Pools]::new($org, $token)
        $this.Agents = [Agents]::new($org, $token)
        $this.Artifacts = [Artifacts]::new($org, $project, $token)
    }
}

class BuildConfiguration {

    [PSCustomObject] Import([string] $configFile) {
        if (-not (Test-Path -Path $configFile -PathType Leaf)) {
          throw [System.InvalidOperationException]::new("Configuration file $configFile is missing")
        }

        $config = Get-Content $configFile | ConvertFrom-Json

        if (-not $config) {
          throw [System.InvalidOperationException]::new("Failed to load configuration file $configFile")
        }

        $dotnetPlatforms = $config.DOTNET_PLATFORMS.Split(' ', [StringSplitOptions]::RemoveEmptyEntries)
        Write-Host "##vso[task.setvariable variable=DOTNET_PLATFORMS;isOutput=true]$dotnetPlatforms"
        foreach ($platform in $dotnetPlatforms) {
            $variableName = "INCLUDE_DOTNET_$($platform.ToUpper())"
            $variableValue = $config.$variableName
            Write-Host "##vso[task.setvariable variable=$variableName;isOutput=true]$variableValue"

            $variableName = "$($platform.ToUpper())_NUGET_VERSION_NO_METADATA"
            $variableValue = $config.$variableName
            Write-Host "##vso[task.setvariable variable=$variableName;isOutput=true]$variableValue"
        }

        return $config
    }

    [PSCustomObject] Create([bool] $addTags, [string] $configFile) {
        # we are going to use a custom object to store all the configuration of the build, this later
        # will be uploaded as an artifact so that it can be easily shared with the cascade pipelines
        $configuration = [PSCustomObject]@{
          BuildReason = "$Env:BUILD_REASON"
          BuildSourceBranchName = "$Env:BUILD_SOURCEBRANCHNAME"
          BuildSourceBranch = "$Env:BUILD_SOURCEBRANCH"
          BuildId = "$Env:BUILD_BUILDID"
          DOTNET_PLATFORMS = "$Env:CONFIGURE_PLATFORMS_DOTNET_PLATFORMS"
        }

        # For each .NET platform we support, add a INCLUDE_DOTNET_<platform> variable specifying whether that platform is enabled or not.
        $dotnetPlatforms = $configuration.DOTNET_PLATFORMS.Split(' ', [StringSplitOptions]::RemoveEmptyEntries)
        foreach ($platform in $dotnetPlatforms) {
            $variableName = "INCLUDE_DOTNET_$($platform.ToUpper())"
            $variableValue = [Environment]::GetEnvironmentVariable("CONFIGURE_PLATFORMS_$variableName")
            $configuration | Add-Member -NotePropertyName $variableName -NotePropertyValue $variableValue

            $variableName = "$($platform.ToUpper())_NUGET_VERSION_NO_METADATA"
            $variableValue = [Environment]::GetEnvironmentVariable("CONFIGURE_PLATFORMS_$variableName")
            $configuration | Add-Member -NotePropertyName $variableName -NotePropertyValue $variableValue
        }

        # calculate the commit to later share it with the cascade pipelines
        if ($Env:BUILD_REASON -eq "PullRequest") {
            $changeId = $configuration.BuildSourceBranch.Replace("refs/pull/", "").Replace("/merge", "")
        } else {
            $changeId = $Env:BUILD_SOURCEVERSION
        }

        $configuration | Add-Member -NotePropertyName Commit -NotePropertyValue $changeId

        # the following list will be used to track the tags and set them in VSTS to make the monitoring person life easier
        [System.Collections.Generic.List[string]]$tags = @()

        if ($configuration.BuildReason -eq "Schedule") {
            $tags.Add("cronjob")
        }

        if ($configuration.BuildReason -eq "PullRequest" -or (($configuration.BuildReason -eq "Manual") -and ($configuration.BuildSourceBranchName -eq "merge")) ) {
          Write-Host "Configuring build from PR."
          # This is an interesting step, we do know we are dealing with a PR, but we need the PR id to
          # be able to get the labels, the buildSourceBranch follows the pattern: refs/pull/{ChangeId}/merge
          # we could use a regexp but then we would have two problems instead of one
          $changeId = $configuration.BuildSourceBranch.Replace("refs/pull/", "").Replace("/merge", "")

          # add a var with the change id, which can be later consumed by some of the old scripts from
          # jenkins
          Write-Host "##vso[task.setvariable variable=pr_number;isOutput=true]$changeId"

          $prInfo = Get-GitHubPRInfo -ChangeId $changeId
          Write-Host $prInfo

          # make peoples life better, loop over the labels and add them as tags in the vsts build
          foreach ($labelInfo in $prInfo.labels) {
            $labelName = $labelInfo.name
            Write-Host "Found label $labelName"
            $tags.Add($labelName)
          }
          # special tag, we want to know if we are using a pr
          $tags.Add("prBuild")

          # special tag, lets add the target branch, will be useful to the users
          $ref = $prInfo.base.ref
          $tags.Add("$ref")

          # set output variables based on the git labels
          $labelsOfInterest = @(
            "build-package",
            "skip-packages",
            "skip-nugets",
            "skip-signing",
            "run-sample-tests",
            "skip-packaged-macos-tests",
            "run-packaged-macos-tests",
            "skip-api-comparison",
            "run-windows-tests",
            "skip-windows-tests",
            "skip-all-tests"
          )

          foreach ($l in $labelsOfInterest) {
            $labelPresent = 1 -eq ($prInfo.labels | Where-Object { $_.name -eq "$l"}).Count
            # We need to replace dashes with underscores, because bash can't access an environment variable with a dash in the name.
            $lbl = $l.Replace('-', '_')
            Write-Host "##vso[task.setvariable variable=$lbl;isOutput=true]$labelPresent"
          }

          Write-Host "##vso[task.setvariable variable=prBuild;isOutput=true]True"
        } else {
          if ($tags.Contains("cronjob")) {
            # debug so that we do know why we do not have ciBuild
            Write-Debug "Skipping the tag 'ciBuild' because we are dealing with a translation build."
          } else  {
            $tags.Add("ciBuild")
          }
          # set the name of the branch under build
          $tags.Add("$($configuration.BuildSourceBranchName)")
          Write-Host "##vso[task.setvariable variable=prBuild;isOutput=true]False"
        }
        # Remove empty entries
        $tags = $tags | Where {$_}
        if ($addTags) {
            Set-BuildTags -Tags $tags.ToArray()
        }
        # add the tags and serialize to json
        $configuration | Add-Member -NotePropertyName Tags -NotePropertyValue $tags

        $jsonConfiguration = $configuration | ConvertTo-Json

        Write-Host "Build configuration:"
        Write-Host $jsonConfiguration

        if ($configFile) {
            Write-Host "Writing configuration to: $configFile"
            Set-Content -Path $configFile -Value $jsonConfiguration
        }

        return $configuration
    }
}

function New-VSTS {
    param
    (
        [Parameter(Mandatory)]
        [String]
        $Org,

        [Parameter(Mandatory)]
        [String]
        $Project,

        [Parameter(Mandatory)]
        [String]
        $Token
    )
    return [Vsts]::new($Org, $Project, $Token)
}


<#
    .SYNOPSIS
        Returns the uri to be used for the VSTS rest API.
#>
function Get-BuildUrl {
    $targetUrl = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI + "$Env:SYSTEM_TEAMPROJECT/_apis/build/builds/" + $Env:BUILD_BUILDID + "?api-version=6.0"
    return $targetUrl
}

<#
    .SYNOPSIS
        Returns the uri to be used for the VSTS rest API for tags.
#>
function Get-TagsRestAPIUrl {
    param
    (
        [Parameter(Mandatory)]
        [String]
        $Tag
    )

    $targetUrl = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI + "$Env:SYSTEM_TEAMPROJECT/_apis/build/builds/" + $Env:BUILD_BUILDID + "/tags/" + $Tag + "?api-version=6.0"
    return $targetUrl
}

<#
    .SYNOPSIS
        Returns the auth heater to use with the REST API of VSTS.
#>
function Get-AuthHeader([string] $AccessToken)
{
    # User name can be anything. It is the personal access token (PAT) token that matters.
    $user = "AnyUser"
    $base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $user, $AccessToken)))
    $headers = @{Authorization = "Basic {0}" -f $base64AuthInfo}

    return $headers
}

<#
    .SYNOPSIS
        Cancels the pipeline and no other steps of job will be executed.

    .EXAMPLE
        Stop-Pipeline

    .NOTES
        The cmdlet depends on the following environment variables. If they are not present
        an InvalidOperationException will be thrown.

        * SYSTEM_TEAMFOUNDATIONCOLLECTIONURI: Contains the full uri of the VSTS for the team.
        * SYSTEM_TEAMPROJECT: Contains the name of the team in VSTS.
        * BUILD_BUILDID: The id of the build to cancel.
        * ACCESSTOKEN: The PAT used to be able to perform the rest call to the VSTS API.
#>
function Stop-Pipeline {
    # assert that all the env vars that are needed are present, else we do have an error
    $envVars = @{
        "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI;
        "SYSTEM_TEAMPROJECT" = $Env:SYSTEM_TEAMPROJECT;
        "BUILD_BUILDID" = $Env:BUILD_BUILDID;
        "ACCESSTOKEN" = $Env:ACCESSTOKEN
    }

    foreach ($key in $envVars.Keys) {
        if (-not($envVars[$key])) {
            Write-Debug "Environment variable missing: $key"
            throw [System.InvalidOperationException]::new("Environment variable missing: $key")
        }
    }

    $url = Get-BuildUrl

    $headers = Get-AuthHeader -AccessToken $Env:ACCESSTOKEN

    $payload = @{
        status = "Cancelling"
    }

    return Invoke-RestMethod -Uri $url -Headers $headers -Method "PATCH" -Body ($payload | ConvertTo-json) -ContentType 'application/json'
}

<#
    .SYNOPSIS
        Allows to set the final status of the pipeline.

    .EXAMPLE
        Set-PipelineResult  "failed"

    .NOTES
        The cmdlet depends on the following environment variables. If they are not present
        an InvalidOperationException will be thrown.

        * SYSTEM_TEAMFOUNDATIONCOLLECTIONURI: Contains the full uri of the VSTS for the team.
        * SYSTEM_TEAMPROJECT: Contains the name of the team in VSTS.
        * BUILD_BUILDID: The id of the build to cancel.
        * ACCESSTOKEN: The PAT used to be able to perform the rest call to the VSTS API.

        The valid values of status are:
        * "canceled" The build was canceled before starting.
        * "failed" The build completed unsuccessfully.
        * "none" No result
        * "partiallySucceeded" The build completed compilation successfully but had other errors.
        * "succeeded" The build completed successfully.
#>
function Set-PipelineResult {
    param
    (
        [Parameter(Mandatory)]
        [String]
        [ValidateScript({
            $("canceled", "failed", "none", "partiallySucceeded", "succeeded").Contains($_) # validate that the status is in the range of valid values
        })]
        $Status
    )

    # assert that all the env vars that are needed are present, else we do have an error
    $envVars = @{
        "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI;
        "SYSTEM_TEAMPROJECT" = $Env:SYSTEM_TEAMPROJECT;
        "BUILD_BUILDID" = $Env:BUILD_BUILDID;
        "ACCESSTOKEN" = $Env:ACCESSTOKEN
    }

    foreach ($key in $envVars.Keys) {
        if (-not($envVars[$key])) {
            Write-Debug "Environment variable missing: $key"
            throw [System.InvalidOperationException]::new("Environment variable missing: $key")
        }
    }

    $url = Get-BuildUrl

    $headers = Get-AuthHeader -AccessToken  $Env:ACCESSTOKEN

    $payload = @{
        result = $Status
    }

    return Invoke-RestMethod -Uri $url -Headers $headers -Method "PATCH" -Body ($payload | ConvertTo-json) -ContentType 'application/json'
}

function Set-BuildTags {
    param
    (
        [String[]]
        $Tags
    )

    # Remove empty entries
    $Tags = $Tags | Where {$_}

    # No need to do anything if there aren't any tags
    Write-Host "Found $($Tags.length) tag(s): $([String]::Join(',', $Tags))"
    if ($Tags.length -eq 0) {
        return
    }

    $envVars = @{
        "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI;
        "SYSTEM_TEAMPROJECT" = $Env:SYSTEM_TEAMPROJECT;
        "BUILD_BUILDID" = $Env:BUILD_BUILDID;
        "ACCESSTOKEN" = $Env:ACCESSTOKEN
    }

    foreach ($key in $envVars.Keys) {
        if (-not($envVars[$key])) {
            throw [System.InvalidOperationException]::new("Environment variable missing: $key")
        }
    }

    # there is an api to just do one request, but it is not clear what should the body be, and we are trying and failing, ergo, use
    # the API that sets one tag at at time.
    # This is why people should write documentation, now I'm being  annoying with the tags

    $headers = Get-AuthHeader -AccessToken  $Env:ACCESSTOKEN

    foreach ($t in $Tags) {
        $url = Get-TagsRestAPIUrl -Tag $t
        Write-Debug "Uri is $url"

        Invoke-RestMethod -Uri $url -Headers $headers -Method "PUT"  -ContentType 'application/json'
    }
}

function Get-YamlPreview {
    param (
        [String]
        $Org,

        [String]
        $Project,

        [String]
        $AccessToken,

        [String]
        $PipelineId,

        [String]
        $Branch,

        [String]
        $OutputFile
    )

    $headers = Get-AuthHeader -AccessToken  $AccessToken

    # create the payload, this payload will point to the correct branch of the repo we want to work with, the repository is always 'self'
    $payload=@{
        "previewRun"=$true
        "resources"=@{
            "repositories"=@{
                "self"=@{
                    "refName"="refs/heads/$Branch"
                }
            }
        }
    }
    $body = ConvertTo-Json $payload -Depth 100

    $url="https://dev.azure.com/$Org/$Project/_apis/pipelines/$PipelineId/preview?api-version=7.1-preview.1"
    try {
        $response=Invoke-RestMethod -Uri $url -Headers $headers -Method "POST"  -ContentType 'application/json' -Body $body
    } catch {
        Write-Host $_
    }
    Set-Content -Path $OutputFile -Value $response.finalYaml  
}

function New-BuildConfiguration {
    param
    (
        [bool]
        $AddTags = $true,

        [string]
        $ConfigFile = ''
    )
    $buildConfiguration = [BuildConfiguration]::new()
    return $buildConfiguration.Create($AddTags, $ConfigFile)
}

function Import-BuildConfiguration {
    param
    (
        [Parameter(Mandatory)]
        [string]
        $ConfigFile
    )
    $buildConfiguration = [BuildConfiguration]::new()
    return $buildConfiguration.Import($ConfigFile)
}

# export public functions, other functions are private and should not be used ouside the module.
Export-ModuleMember -Function Stop-Pipeline
Export-ModuleMember -Function Set-PipelineResult
Export-ModuleMember -Function Set-BuildTags
Export-ModuleMember -Function New-VSTS
Export-ModuleMember -Function New-BuildConfiguration
Export-ModuleMember -Function Import-BuildConfiguration
Export-ModuleMember -Function Get-YamlPreview
