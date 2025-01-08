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

    # list of the default variables we are interested in
    static [string[]] $defaultBuildVariables = $(
        "BUILD_BUILDID",
        "BUILD_BUILDNUMBER",
        "BUILD_BUILDURI",
        "BUILD_BINARIESDIRECTORY",
        "BUILD_DEFINITIONNAME",
        "BUILD_REASON",
        "BUILD_REPOSITORY_ID",
        "BUILD_REPOSITORY_NAME",
        "BUILD_REPOSITORY_PROVIDER",
        "BUILD_REPOSITORY_URI",
        "BUILD_SOURCEBRANCH",
        "BUILD_SOURCEBRANCHNAME",
        "BUILD_SOURCEVERSION"
    )

    <#
        .SYNOPSIS
            Stores the default variables in the current buld as PARENT_BUILD_* in the
            configuration object. This allows cascading pipelines to access the configuration
            of the pipeline that triggered them.
    #>
    [void] StoreParentBuildVariables ([PSCustomObject] $configuration) {
        Write-Debug ("=> StoreParentBuildVariables")
        foreach ($buildVariable in [BuildConfiguration]::defaultBuildVariables) {
            $variableName = "PARENT_BUILD_$buildVariable"
            $variableValue = [Environment]::GetEnvironmentVariable($buildVariable)
            if ($variableValue) {
                Write-Debug "$variableName = $variableValue"
                $configuration | Add-Member -NotePropertyName $variableName -NotePropertyValue $variableValue
            } else {
                Write-Debug "$variableName not found."
            }
        }
    }

    <#
        .SYNOPSIS
            Exports the default variables in the current buld as PARENT_BUILD_* from the
            configuration object.
    #>
    [void] ExportParentBuildVariables ([PSCustomObject] $configuration) {
        Write-Debug ("=> ExportParentBuildVariables")
        foreach ($buildVariable in [BuildConfiguration]::defaultBuildVariables) {
            $variableName = "PARENT_BUILD_$buildVariable"
            $variableValue = $configuration.$variableName
            if ($variableValue) {
                Write-Debug "$variableName = $variableValue"
                Write-Host "##vso[task.setvariable variable=$variableName;isOutput=true]$variableValue"
            } else {
                Write-Debug "$variableName not found."
            }
        }
    }

    static [string[]] $labelsOfInterest = $(
        "build-package",
        "skip-packages",
        "skip-nugets",
        "skip-signing",
        "skip-packaged-macos-tests",
        "run-packaged-macos-tests",
        "skip-api-comparison",
        "run-windows-tests",
        "skip-windows-tests",
        "skip-all-tests"
    )

    [void] SetLabelsFromPR ([PSCustomObject] $prInfo, [bool]$isPR) {
        if ($prInfo) {
            Write-Deubg "Setting VSTS labels from $($prInfo.labels)"
            foreach ($l in [BuildConfiguration]::labelsOfInterest) {
                $labelPresent = 1 -eq ($prInfo.labels | Where-Object { $_.name -eq "$l"}).Count
                # We need to replace dashes with underscores, because bash can't access an environment variable with a dash in the name.
                $lbl = $l.Replace('-', '_')
                Write-Host "##vso[task.setvariable variable=$lbl;isOutput=true]$labelPresent"
            }
        } else {
            Write-Debug "Not setting PR labels because there was not info provided."
        }

        # set if the build is a PR or not
        if ($isPR) {
          Write-Host "##vso[task.setvariable variable=prBuild;isOutput=true]True"
        } else {
          Write-Host "##vso[task.setvariable variable=prBuild;isOutput=true]False"
        }
    }

    <#
        .SYNOPSIS
            Retrieve the change id and export it as an enviroment variable.
    #>
    [string] ExportPRId ([object] $configuration) {
        # This is an interesting step, we do know we are dealing with a PR, but we need the PR id to
        # be able to get the labels, the buildSourceBranch follows the pattern: refs/pull/{ChangeId}/merge
        # we could use a regexp but then we would have two problems instead of one
        $prId = $null
        if ($configuration.PARENT_BUILD_BUILD_SOURCEBRANCH) {
            # there are two possible situations, the build is a PR manually triggered or a PR triggered by a commit to a
            # dev/* branch. In the first case we can get the PR id from the source branch, in the second case we need to
            # get the PR id associated to the current commit. 
            if ($configuration.PARENT_BUILD_BUILD_SOURCEBRANCH.StartsWith("refs/pull")) {
                Write-Host "Getting the change id from the parent build source branch"
                $prId = $configuration.PARENT_BUILD_BUILD_SOURCEBRANCH.Replace("refs/pull/", "").Replace("/merge", "")
            } elseif ($Env:BUILD_SOURCEBRANCH.StartsWith("refs/pull")) {
                Write-Host "Getting the change id from the current build source branch"
                $prId = "$Env:BUILD_SOURCEBRANCH".Replace("refs/pull/", "").Replace("/merge", "")
            } else {
                Write-Host "Getting the change id from the current build source version with the Github API"
                # use the github command to retrieve the associate PR id
                $prIDs = Get-GitHubPRsForHash -Hash $configuration.PARENT_BUILD_BUILD_SOURCEVERSION
                Write-Host "PR IDs: $prIDs"
                if ($prIDs.Length -gt 0) {
                    $prId = $prIDs[0]
                }
            }
        }

        # we can always fail (regexp error or not env varaible)
        if ($prId) {
            # add a var with the change id, which can be later consumed by some of the old scripts from
            # jenkins
            Write-Host "##vso[task.setvariable variable=pr_number;isOutput=true]$prId"
        } else {
            Write-Debug "Not setting the change id because it could not be calculated."
        }
        Write-Host "Change id: $prId"
        return $prId
    }

    [PSCustomObject] Import([string] $configFile) {
        if (-not (Test-Path -Path $configFile -PathType Leaf)) {
          throw [System.InvalidOperationException]::new("Configuration file $configFile is missing")
        }

        $config = Get-Content $configFile | ConvertFrom-Json

        if (-not $config) {
          throw [System.InvalidOperationException]::new("Failed to load configuration file $configFile")
        }

        # load the variables from the config and export them to be accessable from others
        $this.ExportParentBuildVariables($config)

        $dotnetPlatforms = $config.DOTNET_PLATFORMS.Split(' ', [StringSplitOptions]::RemoveEmptyEntries)
        Write-Host "##vso[task.setvariable variable=DOTNET_PLATFORMS;isOutput=true]$dotnetPlatforms"
        foreach ($platform in $dotnetPlatforms) {
            $variableName = "INCLUDE_DOTNET_$($platform.ToUpper())"
            $variableValue = $config.$variableName
            Write-Host "##vso[task.setvariable variable=$variableName;isOutput=true]$variableValue"

            $variableName = "$($platform.ToUpper())_NUGET_VERSION_NO_METADATA"
            $variableValue = $config.$variableName
            Write-Host "##vso[task.setvariable variable=$variableName;isOutput=true]$variableValue"

            $variableName = "$($platform.ToUpper())_NUGET_SDK_NAME"
            $variableValue = $config.$variableName
            Write-Host "##vso[task.setvariable variable=$variableName;isOutput=true]$variableValue"

            $variableName = "$($platform.ToUpper())_NUGET_REF_NAME"
            $variableValue = $config.$variableName
            Write-Host "##vso[task.setvariable variable=$variableName;isOutput=true]$variableValue"

            $variableName = "DOTNET_$($platform.ToUpper())_RUNTIME_IDENTIFIERS"
            $variableValue = $config.$variableName
            Write-Host "##vso[task.setvariable variable=$variableName;isOutput=true]$variableValue"

            $rids = $variableValue.Split(' ', [StringSplitOptions]::RemoveEmptyEntries)
            foreach ($rid in $rids) {
                $variableName = "$($rid)_NUGET_RUNTIME_NAME"
                $variableValue = $config.$variableName
                Write-Host "##vso[task.setvariable variable=$variableName;isOutput=true]$variableValue"
            }
        }

        return $config
    }

    [PSCustomObject] Update([string] $configKey, [string] $configValue, [string] $configFile) {
        if (-not (Test-Path -Path $configFile -PathType Leaf)) {
          throw [System.InvalidOperationException]::new("Configuration file $configFile is missing")
        }

        $config = Get-Content $configFile | ConvertFrom-Json

        if (-not $config) {
          throw [System.InvalidOperationException]::new("Failed to load configuration file $configFile")
        }

        $config | Add-Member -NotePropertyName $configKey -NotePropertyValue $configValue

        $jsonConfiguration = $config | ConvertTo-Json

        Write-Host "Build configuration:"
        Write-Host $jsonConfiguration

        if ($configFile) {
            Write-Host "Writing configuration to: $configFile"
            Set-Content -Path $configFile -Value $jsonConfiguration
        }

        return $config
    }


    [PSCustomObject] Create([bool] $addTags, [string] $configFile) {
        # we are going to use a custom object to store all the configuration of the build, this later
        # will be uploaded as an artifact so that it can be easily shared with the cascade pipelines, we will
        # uses a special prefix for the default variable names so that we do not step on the cascading pipeline
        # settings

        $configuration = [PSCustomObject]@{
          DOTNET_PLATFORMS = "$Env:CONFIGURE_PLATFORMS_DOTNET_PLATFORMS"
        }

        $this.StoreParentBuildVariables($configuration)

        # For each .NET platform we support, add a INCLUDE_DOTNET_<platform> variable specifying whether that platform is enabled or not.
        $dotnetPlatforms = $configuration.DOTNET_PLATFORMS.Split(' ', [StringSplitOptions]::RemoveEmptyEntries)
        foreach ($platform in $dotnetPlatforms) {
            $variableName = "INCLUDE_DOTNET_$($platform.ToUpper())"
            $variableValue = [Environment]::GetEnvironmentVariable("CONFIGURE_PLATFORMS_$variableName")
            $configuration | Add-Member -NotePropertyName $variableName -NotePropertyValue $variableValue

            $variableName = "$($platform.ToUpper())_NUGET_VERSION_NO_METADATA"
            $variableValue = [Environment]::GetEnvironmentVariable("CONFIGURE_PLATFORMS_$variableName")
            $configuration | Add-Member -NotePropertyName $variableName -NotePropertyValue $variableValue

            $variableName = "$($platform.ToUpper())_NUGET_SDK_NAME"
            $variableValue = [Environment]::GetEnvironmentVariable("CONFIGURE_PLATFORMS_$variableName")
            $configuration | Add-Member -NotePropertyName $variableName -NotePropertyValue $variableValue

            $variableName = "$($platform.ToUpper())_NUGET_REF_NAME"
            $variableValue = [Environment]::GetEnvironmentVariable("CONFIGURE_PLATFORMS_$variableName")
            $configuration | Add-Member -NotePropertyName $variableName -NotePropertyValue $variableValue

            $variableName = "DOTNET_$($platform.ToUpper())_RUNTIME_IDENTIFIERS"
            $variableValue = [Environment]::GetEnvironmentVariable("CONFIGURE_PLATFORMS_$variableName")
            $configuration | Add-Member -NotePropertyName $variableName -NotePropertyValue $variableValue

            $rids = $variableValue.Split(' ', [StringSplitOptions]::RemoveEmptyEntries)
            foreach ($rid in $rids) {
                $variableName = "$($rid)_NUGET_RUNTIME_NAME"
                $variableValue = [Environment]::GetEnvironmentVariable("CONFIGURE_PLATFORMS_$variableName")
                $configuration | Add-Member -NotePropertyName $variableName -NotePropertyValue $variableValue
            }
        }

        $xamarinPlatforms = @("ios", "macos", "tvos", "maccatalyst")

        # add all the include platforms as well as the nuget os version
        foreach ($platform in $xamarinPlatforms) {
            $variableName = "INCLUDE_$($platform.ToUpper())"
            $variableValue = [Environment]::GetEnvironmentVariable("$variableName")
            $configuration | Add-Member -NotePropertyName $variableName -NotePropertyValue $variableValue

            $variableName = "$($platform.ToUpper())__NUGET_OS_VERSION"
            $variableValue = [Environment]::GetEnvironmentVariable("$variableName")
            $configuration | Add-Member -NotePropertyName $variableName -NotePropertyValue $variableValue
        }

        # calculate the commit to later share it with the cascade pipelines
        if ($Env:BUILD_REASON -eq "PullRequest") {
            $changeId = $configuration.PARENT_BUILD_BUILD_SOURCEBRANCH.Replace("refs/pull/", "").Replace("/merge", "")
        } else {
            $changeId = $Env:BUILD_SOURCEVERSION
        }

        $configuration | Add-Member -NotePropertyName Commit -NotePropertyValue $changeId

        # the following list will be used to track the tags and set them in VSTS to make the monitoring person life easier
        [System.Collections.Generic.List[string]]$tags = @()

        if ($configuration.BuildReason -eq "Schedule") {
            $tags.Add("cronjob")
        }

        if ($configuration.PARENT_BUILD_BUILD_SOURCEBRANCH.StartsWith ("dev/") -or $configuration.BuildReason -eq "PullRequest" -or (($configuration.BuildReason -eq "Manual") -and ($configuration.PARENT_BUILD_BUILD_SOURCEBRANCH -eq "merge")) ) {
          Write-Host "Configuring build from PR."

          # retrieve the PR data to be able to fwd the labels from github
          $prId = $this.ExportPRId($configuration)
          $prInfo = Get-GitHubPRInfo -ChangeId $prId
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
          $this.SetLabelsFromPR($prInfo, $true)

        } else {
          # thee are not labels to add in a CI build and we will set the build as a ci build.
          $this.SetLabelsFromPR($null, $false)
          if ($tags.Contains("cronjob")) {
            # debug so that we do know why we do not have ciBuild
            Write-Debug "Skipping the tag 'ciBuild' because we are dealing with a translation build."
          } else  {
            $tags.Add("ciBuild")
          }
          # set the name of the branch under build
          $tags.Add("$($configuration.PARENT_BUILD_BUILD_SOURCEBRANCHNAME)")
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

        try {
            Invoke-RestMethod -Uri $url -Headers $headers -Method "PUT"  -ContentType 'application/json'
        } catch {
            Write-Host "Unable to set the build tags"
            Write-Host $_
        }
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

function Edit-BuildConfiguration {
    param
    (

        [Parameter(Mandatory)]
        [string]
        $ConfigKey,

        [Parameter(Mandatory)]
        [string]
        $ConfigValue,

        [Parameter(Mandatory)]
        [string]
        $ConfigFile
    )
    $buildConfiguration = [BuildConfiguration]::new()
    return $buildConfiguration.Update($ConfigKey, $ConfigValue, $ConfigFile)
}

function Find-AzureDevOpsWorkItemWithTitle {
    param
    (
        [Parameter(Mandatory)]
        [string]
        $Title,

        [string]
        $WorkItemType = 'Bug',

        [string]
        $AreaPath = 'DevDiv\VS Client - Runtime SDKs\iOS and Mac'
    )

    $headers = Get-AuthHeader -AccessToken $Env:ACCESSTOKEN

    $url = "$Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI$Env:SYSTEM_TEAMPROJECT/_apis/wit/wiql?api-version=7.1"
    $escapedTitle = $Title.Replace("'", "''")
    $payload = @{
        "query" = "Select [System.Id] FROM WorkItems WHERE [System.State] = 'Active' AND [System.Title] = '$escapedTitle' AND [System.WorkItemType] = '$WorkItemType' AND [System.AreaPath] UNDER '$AreaPath'"
    }
    $body = ConvertTo-Json $payload -Depth 100

    try {
        Write-Host "Uri: $url"
        Write-Host "Body:"
        $body | Out-String | Write-Host

        $response = Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -ContentType 'application/json' -Body $body

        Write-Host "Response:"
        $response | Out-String | Write-Host

        $itemCount = $response.workItems.count
        if ($itemCount -eq 0) {
            Write-Host "No work items found with the given criteria"
            return 0
        }

        $workItemId = $response.workItems[0].id
        Write-Host "Found $itemCount work items with the given criteria, returning id: $workItemId"
        return $workItemId
    } catch {
        Write-Host "Failed to find work item: $_"
    }
    return 0
}

function New-AzureDevOpsWorkItem {
    param
    (
        [Parameter(Mandatory)]
        [string]
        $Message,

        [Parameter(Mandatory)]
        [string]
        $Title,

        [string]
        $WorkItemType = 'Bug',

        [string]
        $AreaPath = 'DevDiv\VS Client - Runtime SDKs\iOS and Mac'
    )

    $headers = Get-AuthHeader -AccessToken $Env:ACCESSTOKEN

    $payload = @(
        @{
            "op" = "add"
            "path" = "/fields/System.Title"
            "from" = $null
            "value" = $Title
        }
        @{
            "op" = "add"
            "path" = "/fields/System.AreaPath"
            "from" = $null
            "value" = $AreaPath
        }
        @{
            "op" = "add"
            "path" = "/fields/Microsoft.VSTS.TCM.ReproSteps"
            "from" = $null
            "value" = $Message
        }
    )

    $body = ConvertTo-Json $payload -Depth 100

    $url = "$Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI$Env:SYSTEM_TEAMPROJECT/_apis/wit/workitems/`$$($WorkItemType)?api-version=7.1"
    try {
        Write-Host "Creating DevOps $WorkItemType with Title=$Title and AreaPath=$AreaPath"
        Write-Host "Uri: $url"
        Write-Host "Body:"
        $body | Out-String | Write-Host
        $response = Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -ContentType 'application/json-patch+json' -Body $body
        Write-Host "Response = $response"
        $workItemId = $response.id
        $workItemUrl = "$Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI$Env:SYSTEM_TEAMPROJECT/_workitems/edit/$workItemId"
        Write-Host "Work Item Url: $workItemUrl"
        return $workItemUrl
    } catch {
        Write-Host "Failed to create work item:"
        $_ | Out-String | Write-Host
        return ""
    }
}

function New-AzureDevOpsWorkItemComment {
    param
    (
        [Parameter(Mandatory)]
        [int]
        $WorkItemId,

        [Parameter(Mandatory)]
        [string]
        $Comment
    )

    $headers = Get-AuthHeader -AccessToken $Env:ACCESSTOKEN

    $payload =
        @{
            "text" = $Comment
        }

    $body = ConvertTo-Json $payload -Depth 100

    $url = "$Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI$Env:SYSTEM_TEAMPROJECT/_apis/wit/workItems/$WorkItemId/comments?api-version=7.1-preview.4"
    try {
        Write-Host "Uri: $url"
        Write-Host "Headers:"
        $headers
        Write-Host "Body:"
        $body
        $response = Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -ContentType 'application/json' -Body $body
        Write-Host "Response:"
        $response
        $workItemUrl = "$Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI$Env:SYSTEM_TEAMPROJECT/_workitems/edit/$workItemId"
        return $workItemUrl
    } catch {
        Write-Host "Failed to add comment to work item:"
        $_ | Out-String | Write-Host
        return ""
    }
}
# export public functions, other functions are private and should not be used ouside the module.
Export-ModuleMember -Function Stop-Pipeline
Export-ModuleMember -Function Set-PipelineResult
Export-ModuleMember -Function Set-BuildTags
Export-ModuleMember -Function New-VSTS
Export-ModuleMember -Function New-BuildConfiguration
Export-ModuleMember -Function Import-BuildConfiguration
Export-ModuleMember -Function Edit-BuildConfiguration
Export-ModuleMember -Function Get-YamlPreview
Export-ModuleMember -Function New-AzureDevOpsWorkItem
Export-ModuleMember -Function New-AzureDevOpsWorkItemComment
Export-ModuleMember -Function Find-AzureDevOpsWorkItemWithTitle

