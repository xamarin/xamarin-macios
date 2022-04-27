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
        $url = "https://dev.azure.com/$($this.Org)/_apis/distributedtask/pools/$($pool.GetID())/agents?agentName=$name&api-version=6.0"
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
        $url = "https://dev.azure.com/$($this.Org)/_apis/distributedtask/pools/$($pool.GetID())/agents?api-version=6.0"
        $headers = Get-AuthHeader($this.Token)
        $agents = Invoke-RestMethod -Uri $url -Headers $headers -Method "GET"  -ContentType 'application/json'
        $result = [System.Collections.ArrayList]@()
        foreach ($a in $agents.value) {
            $result.Add([Agent]::new($a))
        }
        return $result
    }

    [void] SetEnabled($pool, $agent, $isEnabled) {
        if (-not $agent) {
            throw [System.ArgumentNullException]::new("pool")
        }
        $url = "https://dev.azure.com/$($this.Org)/_apis/distributedtask/pools/$($pool.GetID())/agents/$($agent.GetID())?api-version=6.0"
        $headers = Get-AuthHeader($this.Token)
        $payload = @{
            enabled = $isEnabled
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
        $this.Agents = [Artifacts]::new($org, $project, $token)
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

    # there is an api to just do one request, but it is not clear what should the body be, and we are trying and failing, ergo, use
    # the API that sets one tag at at time.
    # This is why people should write documentation, now I'm being  annoying with the tags

    $headers = Get-AuthHeader -AccessToken  $Env:ACCESSTOKEN

    foreach ($t in $Tags) {
        $url = Get-TagsRestAPIUrl -Tag $t
        Write-Host "Uri is $url"

        Invoke-RestMethod -Uri $url -Headers $headers -Method "PUT"  -ContentType 'application/json'
    }
}

# export public functions, other functions are private and should not be used ouside the module.
Export-ModuleMember -Function Stop-Pipeline
Export-ModuleMember -Function Set-PipelineResult
Export-ModuleMember -Function Set-BuildTags
Export-ModuleMember -Function New-VSTS
