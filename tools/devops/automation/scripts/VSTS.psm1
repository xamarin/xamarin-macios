<#
    .SYNOPSIS
        Returns the uri to be used for the VSTS rest API.
#>
function Get-BuildUrl {
    $targetUrl = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI + "$Env:SYSTEM_TEAMPROJECT/_apis/build/builds/" + $Env:BUILD_BUILDID + "?api-version=5.1"
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
        * SYSTEM_ACCESSTOKEN: The PAT used to be able to perform the rest call to the VSTS API.
#>
function Stop-Pipeline {
    # assert that all the env vars that are needed are present, else we do have an error
    $envVars = @{
        "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI;
        "SYSTEM_TEAMPROJECT" = $Env:SYSTEM_TEAMPROJECT;
        "BUILD_BUILDID" = $Env:BUILD_BUILDID;
        "SYSTEM_ACCESSTOKEN" = $Env:SYSTEM_ACCESSTOKEN
    }

    foreach ($key in $envVars.Keys) {
        if (-not($envVars[$key])) {
            Write-Debug "Environment variable missing: $key"
            throw [System.InvalidOperationException]::new("Environment variable missing: $key")
        }
    }

    $url = Get-BuildUrl

    $headers = @{
        Authorization = ("Bearer {0}" -f $Env:SYSTEM_ACCESSTOKEN)
    }

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
        * SYSTEM_ACCESSTOKEN: The PAT used to be able to perform the rest call to the VSTS API.

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
        "SYSTEM_ACCESSTOKEN" = $Env:SYSTEM_ACCESSTOKEN
    }

    foreach ($key in $envVars.Keys) {
        if (-not($envVars[$key])) {
            Write-Debug "Environment variable missing: $key"
            throw [System.InvalidOperationException]::new("Environment variable missing: $key")
        }
    }

    $url = Get-BuildUrl

    $headers = Get-AuthHeader -AccessToken  $Env:SYSTEM_ACCESSTOKEN

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
        "SYSTEM_ACCESSTOKEN" = $Env:SYSTEM_ACCESSTOKEN
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

    $headers = Get-AuthHeader -AccessToken  $Env:SYSTEM_ACCESSTOKEN

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
