# the following is a hack around the fact that pwsh does not handle well the using 
# form a relative path in vsts
$moduleName = "$PSScriptRoot\\StaticPages.psm1"  # windows path separators work on unix and windows
$scriptBody = "using module $ModuleName"
$script = [ScriptBlock]::Create($scriptBody)
. $script

<#
    .SYNOPSIS
        Simple retry block to workaround certain issues with the webservices that cannot handle the load.

    .PARAMETER Request
        The request to be performed and retried if failed.

    .PARAMETER Retries
        The number of times the we will retry to perform the request.
#>
function Invoke-Request {
    param (
        [scriptblock]
        $Request,

        [int]
        $Retries=5
    )
    $count = 0
    do {
        try {
            # that & is important, tells pwsh to execute the script block, else you simple returns the block itself
            return & $Request
        } catch {
            if ($count -gt $Retries) {
                # notify and throw
                Write-Host "Could not perform request after $Retries attempts."
                throw $_.Exception
            } else {
                $count = $count + 1
                $seconds = 5 * $count
                Write-Host "Error performing request trying in $seconds seconds"
                Write-Host "Exception was:"
                Write-Host "$($_.Exception)"
                Write-Host "Response was:"
                Write-Host "$($_.Exception.Response)"
                Start-Sleep -Seconds $seconds
            }
        }

    } while ($true)
}

<#
    .SYNOPSIS
        Returns the target url to be used when setting the status. The target url allows users to get back to the CI event that updated the status.
#>
function Get-TargetUrl {
    $targetUrl = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI + "$Env:SYSTEM_TEAMPROJECT/_build/index?buildId=$Env:BUILD_BUILDID&view=ms.vss-test-web.test-result-details"
    return $targetUrl
}

<#
    .SYNOPSIS
        Returns the url to the Html Report index page stored in xamarin-storage.
#>
function Get-XamarinStorageIndexUrl {
    param (
        [Parameter(Mandatory)]
        [String]
        $Path
    )

    return "http://xamarin-storage/$Path/jenkins-results/tests/index.html"
}

<#
    .SYNOPSIS
        Sets a new status in github for the current build.
    .DESCRIPTION

    .PARAMETER Status
        The status value to be set in GitHub. The available values are:

        * error
        * failure
        * pending
        * success

        If the wrong value is passed a validation error with be thrown.

    .PARAMETER Description
        The description that will be added with the status update. This allows us to add a human readable string
        to understand why the status was updated.
    
    .PARAMETER Context
        The context to be used. A status can contain several contexts. The context must be passed to associate
        the status with a specific event.

    .EXAMPLE
        Set-GitHubStatus -Status "error" -Description "Not enough free space in the host." -Context "VSTS iOS device tests."

    .NOTES
        This cmdlet depends on the following environment variables. If one or more of the variables is missing an
        InvalidOperationException will be thrown:

        * SYSTEM_TEAMFOUNDATIONCOLLECTIONURI: The uri of the vsts collection. Needed to be able to calculate the target url.
        * SYSTEM_TEAMPROJECT: The team project executing the build. Needed to be able to calculate the target url.
        * BUILD_BUILDID: The current build id. Needed to be able to calculate the target url.
        * BUILD_REVISION: The revision of the current build. Needed to know the commit whose status to change.
        * GITHUB_TOKEN: OAuth or PAT token to interact with the GitHub API.
#>
function Set-GitHubStatus {
    param
    (
        [Parameter(Mandatory)]
        [String]
        [ValidateScript({
            $("error", "failure", "pending", "success").Contains($_) #validate that the status is in the range of valid values
        })]
        $Status,

        [Parameter(Mandatory)]
        [String]
        $Description,

        [Parameter(Mandatory)]
        [String]
        $Context,

        [String]
        $TargetUrl
    )

    # assert that all the env vars that are needed are present, else we do have an error
    $envVars = @{
        "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI;
        "SYSTEM_TEAMPROJECT" = $Env:SYSTEM_TEAMPROJECT;
        "BUILD_BUILDID" = $Env:BUILD_BUILDID;
        "BUILD_REVISION" = $Env:BUILD_REVISION;
        "BUILD_REASON" = $Env:BUILD_REASON;
        "BUILD_SOURCEBRANCHNAME" = $Env:BUILD_SOURCEBRANCHNAME;
        "GITHUB_TOKEN" = $Env:GITHUB_TOKEN;
    }

    foreach ($key in $envVars.Keys) {
        if (-not($envVars[$key])) {
            Write-Debug "Enviroment varible missing $key"
            throw [System.InvalidOperationException]::new("Environment variable missing: $key")
        }
    }

    if ($Env:BUILD_REASON -eq "PullRequest") {
        # the env var is only provided for PR not for builds.
        $url = "https://api.github.com/repos/xamarin/xamarin-macios/statuses/$Env:SYSTEM_PULLREQUEST_SOURCECOMMITID"
    } else {
        $url = "https://api.github.com/repos/xamarin/xamarin-macios/statuses/$Env:BUILD_REVISION"
    }

    $headers = @{
        Authorization = ("token {0}" -f $Env:GITHUB_TOKEN)
    }

    $requestContext = $Context
    # Check if the status was already set, if it was we will override yet print a message for the user to know this action was done.
    $presentStatuses = Invoke-Request -Request { Invoke-RestMethod -Uri $url -Headers $headers -Method "GET" -ContentType 'application/json' }

    # try to find the status with the same context and make a decision, this is not a dict but an array :/ 
    foreach ($s in $presentStatuses) {
        # we found a status from a previous build that was a success, we do not want to step on it
        if (($s.context -eq $Context) -and ($s.state -eq "success")) {
            Write-Host "WARNING: Found status for $Context because it was already set as a success, overriding result."
        }
    }

    # use the GitHub API to set the status for the given commit
    $detailsUrl = ""
    if ($TargetUrl) {
        $detailsUrl = $TargetUrl
    } else {
        $detailsUrl = Get-TargetUrl
    }
    $payload= @{
        state = $Status
        target_url = $detailsUrl
        description = $Description
        context = $requestContext
    }

    return Invoke-Request -Request { Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -Body ($payload | ConvertTo-json) -ContentType 'application/json' }
}

<#
    .SYNOPSIS
        Add a new comment for the commit on GitHub.

    .PARAMETER Header
        The header to be used in the comment.

    .PARAMETER Description
        A show description to be added in the comment, this will show as a short version of the comment on GitHub.
    
    .PARAMETER Message
        A longer string that contains the full comment message. Will be shown when the comment is expanded.

    .PARAMETER Emoji
        Optional string representing and emoji to be used in the comments header.

    .EXAMPLE
        New-GitHubComment -Header "Tests failed catastrophically" -Emoji ":fire:" -Description "Not enough free space in the host."

    .NOTES
        This cmdlet depends on the following environment variables. If one or more of the variables is missing an
        InvalidOperationException will be thrown:

        * SYSTEM_TEAMFOUNDATIONCOLLECTIONURI: The uri of the vsts collection. Needed to be able to calculate the target url.
        * SYSTEM_TEAMPROJECT: The team project executing the build. Needed to be able to calculate the target url.
        * BUILD_BUILDID: The current build id. Needed to be able to calculate the target url.
        * BUILD_REVISION: The revision of the current build. Needed to know the commit whose status to change.
        * GITHUB_TOKEN: OAuth or PAT token to interact with the GitHub API.
#>
function New-GitHubComment {
    param
    (
        [Parameter(Mandatory)]
        [String]
        $Header,
        
        [Parameter(Mandatory)]
        [String]
        $Description,

        [String]
        $Message,

        [String]
        $Emoji #optionally use an emoji
    )

    # assert that all the env vars that are needed are present, else we do have an error
    $envVars = @{
        "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI;
        "SYSTEM_TEAMPROJECT" = $Env:SYSTEM_TEAMPROJECT;
        "BUILD_DEFINITIONNAME" = $Env:BUILD_DEFINITIONNAME;
        "BUILD_REVISION" = $Env:BUILD_REVISION;
        "BUILD_REASON" = $Env:BUILD_REASON;
        "BUILD_SOURCEBRANCH" = $Env:BUILD_SOURCEBRANCH;
        "GITHUB_TOKEN" = $Env:GITHUB_TOKEN;
    }

    foreach ($key in $envVars.Keys) {
        if (-not($envVars[$key])) {
            Write-Debug "Enviroment varible missing $key"
            throw [System.InvalidOperationException]::new("Environment variable missing: $key")
        }
    }

    $targetUrl = Get-TargetUrl
    # build the message, which will be sent to github, users can use markdown
    $msg = [System.Text.StringBuilder]::new()
    $msg.AppendLine("### $Emoji $Header $Emoji")
    $msg.AppendLine()
    $msg.AppendLine($Description)
    if ($Message) { # only if message is not null or empty
        $msg.AppendLine()
        $msg.AppendLine($Message)
    }
    $msg.AppendLine()
    $msg.AppendLine("[Pipeline]($targetUrl) on Agent $Env:TESTS_BOT") # Env:TESTS_BOT is added by the pipeline as a variable coming from the execute tests job
    $msg.AppendLine("$Env:BUILD_SOURCEVERSIONMESSAGE") # default envars to provide more context to the result

    # if the build was due to PR, we want to write the comment in the PR rather than in the commit 
    if ($Env:BUILD_REASON -eq "PullRequest") {
        # calcualte the change ID which is the PR number 
        $buildSourceBranch = $Env:BUILD_SOURCEBRANCH
        $changeId = $buildSourceBranch.Replace("refs/pull/", "").Replace("/merge", "")
        $url = "https://api.github.com/repos/xamarin/xamarin-macios/issues/$changeId/comments"
    } else {
        $url = "https://api.github.com/repos/xamarin/xamarin-macios/commits/$Env:BUILD_REVISION/comments"
    }

    # github has a max size for the comments to be added in a PR, it can be the case that because we failed so much, that we
    # cannot add the full message, in that case, we add part of it, then a link to a gist with the content.
    $maxLength = 32768
    $body = $msg.ToString()
    if ($body.Length -ge $maxLength) {
        # create a gist with the contents, next, add substring of the message - the length of the info about the gist so that users
        # can click, set that as the body
        $gist =  New-GistWithContent -Description "Build results" -FileName "TestResult.md" -GistContent $body -FileType "md"
        $linkMessage = "The message from CI is too large for the GitHub comments. You can find the full results [here]($gist)."
        $messageLength = $maxLength - ($linkMessage.Length + 2) # +2 is to add a nice space
        $body = $body.Substring(0, $messageLength);
        $body = $body + "\n\n" + $linkMessage
    }

    $payload = @{
        body = $body
    }

    $headers = @{
        Authorization = ("token {0}" -f $Env:GITHUB_TOKEN)
    }

    $request = Invoke-Request -Request { Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -Body ($payload | ConvertTo-Json) -ContentType 'application/json' }
    Write-Host $request
    return $request
}

<#
    .SYNOPSIS
        Add a new comment that contains the result summaries of the test run.

    .PARAMETER Header
        The header to be used in the comment.

    .PARAMETER Description
        A show description to be added in the comment, this will show as a short version of the comment on GitHub.
    
    .PARAMETER Message
        A longer string that contains the full comment message. Will be shown when the comment is expanded.

    .PARAMETER Emoji
        Optional string representing and emoji to be used in the comments header.

    .EXAMPLE
        New-GitHubComment -Header "Tests failed catastrophically" -Emoji ":fire:" -Description "Not enough free space in the host."

    .NOTES
        This cmdlet depends on the following environment variables. If one or more of the variables is missing an
        InvalidOperationException will be thrown:

        * SYSTEM_TEAMFOUNDATIONCOLLECTIONURI: The uri of the vsts collection. Needed to be able to calculate the target url.
        * SYSTEM_TEAMPROJECT: The team project executing the build. Needed to be able to calculate the target url.
        * BUILD_BUILDID: The current build id. Needed to be able to calculate the target url.
        * BUILD_REVISION: The revision of the current build. Needed to know the commit whose status to change.
        * GITHUB_TOKEN: OAuth or PAT token to interact with the GitHub API.
#>
function New-GitHubCommentFromFile {
    param (

        [Parameter(Mandatory)]
        [String]
        $Header,
        
        [Parameter(Mandatory)]
        [String]
        $Description,

        [Parameter(Mandatory)]
        [String]
        [ValidateScript({
            Test-Path -Path $_ -PathType Leaf 
        })]
        $Path,

        [String]
        $Emoji #optionally use an emoji
    )

    # read the file, create a message and use the New-GithubComment function
    $msg = [System.Text.StringBuilder]::new()
    foreach ($line in Get-Content -Path $Path)
    {
        $msg.AppendLine($line)
    }
    return New-GithubComment -Header $Header -Description $Description -Message $msg.ToString() -Emoji $Emoji
}

<#
    .SYNOPSIS
        Test if the current job is successful or not.
#>
function Test-JobSuccess {

    param (
        [Parameter(Mandatory)]
        [String]
        $Status
    )

    # return if the status is one of the failure ones
    return $Status -eq "Succeeded"
}

<# 
    .SYNOPSIS
        Helper function used to create the content in the comment with the APIDiff

    .PARAMETER APIDiff
        The path to the the json that contains the content for the PR API diff.

    .PARAMETER APIGeneratorDiffJson
        The path to the json that contains the content for the generator diffs with stable.

    .PARAMETER APIGeneratorDiff
        The path to the json that contains the content for the generator diffs.
#>
function Write-APIDiffContent {
    param (

        [Parameter(Mandatory)]
        [System.Text.StringBuilder]
        $StringBuilder,

        [String]
        $APIDiff="",

        [string]
        $APIGeneratorDiffJson="",

        [string]
        $APIGeneratorDiff=""
    )

    if ([string]::IsNullOrEmpty($APIDiff)) {
        $StringBuilder.AppendLine("* :warning: API diff urls have not been provided.")
    } else {
        Write-Diffs -StringBuilder $sb -Header "API diff" -APIDiff $APIDiff
    }
    if ([string]::IsNullOrEmpty($APIGeneratorDiffJson)) {
        $StringBuilder.AppendLine("* :warning: API Current PR diff urls have not been provided.")
    } else {
        Write-Diffs -StringBuilder $sb -Header "API Current PR diff" -APIDiff $APIGeneratorDiffJson
    }
    if (-not [string]::IsNullOrEmpty($APIGeneratorDiff)) {
        Write-Host "Parsing Generator diff in path $APIGeneratorDiff"
        if (-not (Test-Path $APIGeneratorDiff -PathType Leaf)) {
            $StringBuilder.AppendLine("* :warning: Path $APIGeneratorDiff was not found!")
        } else {
            $StringBuilder.AppendLine("# Generator diff")
            $StringBuilder.AppendLine("")
            # ugly workaround to get decent new lines
            foreach ($line in Get-Content -Path $APIGeneratorDiff)
            {
                $StringBuilder.AppendLine($line)
            }
            $StringBuilder.AppendLine($apidiffcomments)
        }
    } else {
        $StringBuilder.AppendLine("* :warning: Generator diff comments have not been provided.")
    }
}

<# 
    .SYNOPSIS
        Helper function used to create the content in the comment with the artifacts.

    .PARAMETER Artifacts
        The json that contains all the artifacts.
#>
function Write-Artifacts {

    param (

        [Parameter(Mandatory)]
        [System.Text.StringBuilder]
        $StringBuilder,

        [String]
        $Artifacts=""

    )

    if (-not [string]::IsNullOrEmpty($Artifacts)) {
        Write-Host "Parsing artifacts"
        if (-not (Test-Path $Artifacts -PathType Leaf)) {
            $StringBuilder.AppendLine("Path $Artifacts was not found!")
        } else {
            # read the json file, convert it to an object and add a line for each artifact
            $json =  Get-Content $Artifacts | ConvertFrom-Json
            if ($json.Count -gt 0) {
                $StringBuilder.AppendLine("# Packages generated")
                $StringBuilder.AppendLine("")
                $StringBuilder.AppendLine("<details><summary>View packages</summary>")
                $StringBuilder.AppendLine("") # no new line results in a bad rendering in the links
                foreach ($a in $json) {
                    $url = $a.url
                    if ($url.EndsWith(".pkg") -or $url.EndsWith(".nupkg") -or $url.EndsWith(".msi")) {
                        try {
                            $fileName = $a.url.Substring($a.url.LastIndexOf("/") + 1)
                            Write-Host "Adding link for $fileName"
                            if ($a.url.Contains("notarized")) {
                                $link = "* [$fileName (notarized)]($($a.url))"
                            } else {
                                $link = "* [$fileName]($($a.url))"
                            }
                            $StringBuilder.AppendLine($link)
                        } catch {
                            Write-Host "Could not get file name for url $url"
                        }
                    }
                }
                $StringBuilder.AppendLine("</details>")
            } else {
                $StringBuilder.AppendLine("No packages found.")
            }
        }
    } else {
        Write-Host "Artifacts were not provided."
    }
}

<# 
    .SYNOPSIS
        Add a new comment that contains the summaries to the Html Report as well as set the status accordingly.

    .PARAMETER Context
        The context to be used to link the status and the device test run in the GitHub status API.

    .PARAMETER TestSummaryPath
        The path to the generated test summary.

    .EXAMPLE
        New-GitHubSummaryComment -Context "$Env:CONTEXT" -TestSummaryPath "$Env:SYSTEM_DEFAULTWORKINGDIRECTORY/xamarin/xamarin-macios/tests/TestSummary.md"
    .NOTES
        This cmdlet depends on the following environment variables. If one or more of the variables is missing an
        InvalidOperationException will be thrown:

        * SYSTEM_TEAMFOUNDATIONCOLLECTIONURI: The uri of the vsts collection. Needed to be able to calculate the target url.
        * SYSTEM_TEAMPROJECT: The team project executing the build. Needed to be able to calculate the target url.
        * BUILD_BUILDID: The current build id. Needed to be able to calculate the target url.
        * BUILD_REVISION: The revision of the current build. Needed to know the commit whose status to change.
        * GITHUB_TOKEN: OAuth or PAT token to interact with the GitHub API.

#>
function New-GitHubSummaryComment {
    param (
        [Parameter(Mandatory)]
        [String]
        $Context,

        [Parameter(Mandatory)]
        [String]
        $TestSummaryPath,

        [string]
        $Artifacts="",

        [string]
        $APIDiff="",

        [string]
        $APIGeneratorDiffJson="",

        [string]
        $APIGeneratorDiff="",

        [switch]
        $DeviceTest
    )

    $envVars = @{
        "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI;
        "SYSTEM_TEAMPROJECT" = $Env:SYSTEM_TEAMPROJECT;
        "BUILD_DEFINITIONNAME" = $Env:BUILD_DEFINITIONNAME;
        "BUILD_REVISION" = $Env:BUILD_REVISION;
        "GITHUB_TOKEN" = $Env:GITHUB_TOKEN;
    }

    foreach ($key in $envVars.Keys) {
        if (-not($envVars[$key])) {
            Write-Debug "Environment variable missing: $key"
            throw [System.InvalidOperationException]::new("Environment variable missing: $key")
        }
    }

    $vstsTargetUrl = Get-TargetUrl
    # build the links to provide extra info to the monitoring person, we need to make sure of a few things
    # 1. We do have the xamarin-storage path
    # 2. We did reach the xamarin-storage, stored in the env var XAMARIN_STORAGE_REACHED
    $sb = [System.Text.StringBuilder]::new()
    $sb.AppendLine(); # new line to start the list
    $sb.AppendLine("* [Azure DevOps]($vstsTargetUrl)")
    if ($Env:VSDROPS_INDEX) {
        # we did generate an index with the files in vsdrops
        $sb.AppendLine("* [Html Report (VSDrops)]($Env:VSDROPS_INDEX) [Download]($Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI$Env:SYSTEM_TEAMPROJECT/_apis/build/builds/$Env:BUILD_BUILDID/artifacts?artifactName=HtmlReport-simulator&api-version=6.0&`$format=zip)")
    }

    if (-not $DeviceTest) {
        Write-APIDiffContent -StringBuilder $sb -APIDiff $APIDiff -APIGeneratorDiffJson $APIGeneratorDiffJson -APIGeneratorDiff $APIGeneratorDiff

        Write-Artifacts -StringBuilder $sb -Artifacts $Artifacts
    }

    if (Test-Path $TestSummaryPath -PathType Leaf) { # if present we did get results and add the links, else skip
        $githubPagePrefix = "https://xamarin.github.io/macios.ci"
        if (-not [string]::IsNullOrEmpty($Env:PR_ID)) {
            $staticPageComment = [StaticPages]::new($githubPagePrefix, $Env:PR_ID, $Env:BUILD_BUILDID)
            $staticPageComment.WriteComment($sb)
        }
    }

    $headerLinks = $sb.ToString()
    $request = $null

    # set the context to be "pipeline name (Test run)", example xamarin-macios (Test run)
    $statusContext = "$Env:BUILD_DEFINITIONNAME (Test run)"
    if ($Context -ne "Build") { #special case when we deal with the device tests
        $statusContext = "$Contex - $Env:BUILD_DEFINITIONNAME) (Test run)"
    }

    # make a diff between a PR and a CI build so that users do not get confused.
    $prefix = "";
    if ([string]::IsNullOrEmpty($Env:PR_ID)) {
        $prefix = "[CI Build]"
    } else {
        $prefix = "[PR Build]"
    }

    if (-not (Test-Path $TestSummaryPath -PathType Leaf)) {
        Write-Host "No test summary found"
        Set-GitHubStatus -Status "failure" -Description "$prefix Tests failed catastrophically on $Context (no summary found)." -Context $statusContext
        $request = New-GitHubComment -Header "Tests failed catastrophically on $Context (no summary found)." -Emoji ":fire:" -Description "Result file $TestSummaryPath not found. $headerLinks"
    } else {
        if ($Env:TESTS_JOBSTATUS -eq "") {
            Set-GitHubStatus -Status "error" -Description "Tests didn't execute on $Context." -Context $statusContext
            $request = New-GitHubCommentFromFile -Header "$prefix Tests didn't execute on $Context." -Description "Tests didn't execute on $Context. $headerLinks"  -Emoji ":x:" -Path $TestSummaryPath
        } elseif (Test-JobSuccess -Status $Env:TESTS_JOBSTATUS) {
            Set-GitHubStatus -Status "success" -Description "All tests passed on $Context." -Context $statusContext
            $request = New-GitHubCommentFromFile -Header "$prefix Tests passed on $Context." -Description "Tests passed on $Context. $headerLinks"  -Emoji ":white_check_mark:" -Path $TestSummaryPath
        } else {
            Set-GitHubStatus -Status "error" -Description "Tests failed on $Context." -Context $statusContext
            $request = New-GitHubCommentFromFile -Header "$prefix Tests failed on $Context" -Description "Tests failed on $Context. $headerLinks" -Emoji ":x:" -Path $TestSummaryPath
        }
    }
    return $request
}

function Write-Diffs {
    param (
        [Parameter(Mandatory)]
        [System.Text.StringBuilder]
        $StringBuilder,

        [Parameter(Mandatory)]
        [String]
        $Header,

        [String]
        $APIDiff
    )

    Write-Host "Parsing API diff in path $APIDiff"
    if (-not (Test-Path $APIDiff -PathType Leaf)) {
        $StringBuilder.AppendLine("Path $APIDiff was not found!")
    } else {
        # read the json file, convert it to an object and add a line for each artifact
        $json =  Get-Content $APIDiff | ConvertFrom-Json
        # we are dealing with an object, not a dictionary
        $hasHtmlLinks = "html" -in $json.PSobject.Properties.Name
        $hasMDlinks = "gist" -in $json.PSobject.Properties.Name
        if ($hasHtmlLinks -or $hasMDlinks) {
            $StringBuilder.AppendLine("# $Header")
            Write-Host "Message is '$($json.message)'"
            $StringBuilder.AppendLine($json.message)

            $commonPlatforms = "iOS", "macOS", "tvOS"
            $legacyPlatforms = @{Title="API diff"; Platforms=@($commonPlatforms + "watchOS");}
            $dotnetPlatforms = @{Title="dotnet API diff"; Platforms=@($commonPlatforms + "MacCatalyst").ForEach({"dotnet-" + $_});}
            $dotnetLegacyPlatforms = @{Title="dotnet legacy API diff"; Platforms=@($commonPlatforms).ForEach({"dotnet-legacy-" + $_});}
            $dotnetMaciOSPlatforms = @{Title="dotnet iOS-MacCatalayst API diff"; Platforms=@("macCatiOS").ForEach({"dotnet-" + $_});}
            $platforms = @($legacyPlatforms, $dotnetPlatforms, $dotnetLegacyPlatforms, $dotnetMaciOSPlatforms)

            foreach ($linkGroup in $platforms) {
                $StringBuilder.AppendLine("<details><summary>View $($linkGroup.Title)</summary>")
                $StringBuilder.AppendLine("") # no new line results in a bad rendering in the links
                $htmlLink = ""
                $gistLink = ""

                foreach ($linkPlatform in $linkGroup.Platforms) {
                    $platformHasHtmlLinks = $linkPlatform -in $json.html.PSobject.Properties.Name
                    $platformHasMDlinks = $linkPlatform -in $json.gist.PSobject.Properties.Name

                    # some do not have md, some do not have html
                    if ($platformHasHtmlLinks) {
                        Write-Host "Found html link for $linkPlatform"
                        $htmlLinkUrl = $json.html | Select-Object -ExpandProperty $linkPlatform
                        $htmlLink = "[vsdrops]($htmlLinkUrl)"
                    }

                    if ($platformHasMDlinks) {
                        Write-Host "Found gist link for $linkPlatform"
                        $gistLinkUrl = $json.gist | Select-Object -ExpandProperty $linkPlatform
                        $gistLink = "[gist]($gistLinkUrl)"
                    }

                    if (($htmlLink -eq "") -and ($gistLink -eq "")) {
                        $StringBuilder.AppendLine("* :fire: $linkPlatform :fire: Missing files")
                    } else {
                        # I don't like extra ' ' when we are missing vars, use join
                        $line = @("*", $linkPlatform, $htmlLink, $gistLink) -join " "
                        $StringBuilder.AppendLine($line)
                    }
                }
                $StringBuilder.AppendLine("</details>")
                $StringBuilder.AppendLine("")
            }
            $StringBuilder.AppendLine("")
        } else {
            $StringBuilder.AppendLine("# API diff")
            $StringBuilder.AppendLine("")
            $StringBuilder.AppendLine("**No api diff data found.**")
        }
    }
}

<# 
    .SYNOPSIS
        Get the information of a PR in GitHub.

    .PARAMETER ChangeId
        The Id whose labels we want to retrieve.
#>
function Get-GitHubPRInfo {
    param (
        [Parameter(Mandatory)]
        [String]
        $ChangeId
    )

    $envVars = @{
        "GITHUB_TOKEN" = $Env:GITHUB_TOKEN;
    }

    foreach ($key in $envVars.Keys) {
        if (-not($envVars[$key])) {
            Write-Debug "Environment variable missing: $key"
            throw [System.InvalidOperationException]::new("Environment variable missing: $key")
        }
    }

    $url = "https://api.github.com/repos/xamarin/xamarin-macios/pulls/$ChangeId"

    $request = Invoke-Request -Request { Invoke-RestMethod -Uri $url -Method "GET" -ContentType 'application/json' }
    Write-Host $request
    return $request
}

<#
    .SYNOPSIS
        Class used to represent a single file to be added to a gist.
#>
class GistFile
{
    [ValidateNotNullOrEmpty ()]
    [string]
    $Name
    [ValidateNotNullOrEmpty ()]
    [string]
    $Path
    [ValidateNotNullOrEmpty ()]
    [string]
    $Type

    GistFile ($Name, $Path, $Type) {
        # validate that the path does exist
        if (Test-Path -Path $Path -PathType Leaf) {
            $this.Path = $Path
        } else {
            throw [System.InvalidOperationException]::new("Path could not be found: $Path")
        }
        $this.Name = $Name
        $this.Type = $Type
     }

    [hashtable] ConvertToHashTable () {
        # ugly workaround to get decent new lines
        $file= [System.Text.StringBuilder]::new()
        foreach ($line in Get-Content -Path $this.Path)
        {
            $file.AppendLine($line)
        }

        return @{
            content = $file.ToString()
            filename = $this.Name;
            language = $this.Type;
        }
    }
}

function New-GistWithContent {
    param (

        [ValidateNotNullOrEmpty ()]
        [string]
        $Description, 

        [Parameter(Mandatory)]
        [string]
        $FileName,

        [Parameter(Mandatory)]
        [string]
        $GistContent,

        [Parameter(Mandatory)]
        [string]
        $FileType,

        [switch]
        $IsPublic=$false # default to false, better save than sorry
    )

    $envVars = @{
        "GITHUB_TOKEN" = $Env:GITHUB_TOKEN;
    }

    foreach ($key in $envVars.Keys) {
        if (-not($envVars[$key])) {
            Write-Debug "Environment variable missing: $key"
            throw [System.InvalidOperationException]::new("Environment variable missing: $key")
        }
    }

    # create the hashtable that will contain all the information of all types
    $payload = @{
        description = $Description;
        files = @{
            "$FileName" = @{
                content = $GistContent;
                filename = $FileName
                language = $FileType;
            };
        }; # each file is the name of the file + the hashtable of the data to be used
    }

    # switchs are converted to {\"IsPresent\"=>true} in json :/ and the ternary operator might not be in all machines
    if ($IsPublic) {
        $payload["public"] = $true
    } else {
        $payload["public"] = $false
    }

    $url = "https://api.github.com/gists"
    $payloadJson = $payload | ConvertTo-Json
    Write-Host "Url is $url"
    Write-Host "Payload is $payloadJson"

    $headers = @{
        Accept = "application/vnd.github.v3+json";
        Authorization = ("token {0}" -f $Env:GITHUB_TOKEN);
    } 

    $request = Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -Body $payloadJson -ContentType 'application/json'
    Write-Host $request
    return $request.html_url
}

<# 
    .SYNOPSIS
        Creates a new gist that will contain the given collection of files and returns the urlobject defintion, this
        is usefull when the 'using' statement generates problems.
#>
function New-GistObjectDefinition {
    param (

        [ValidateNotNullOrEmpty ()]
        [string]
        $Name, 

        [ValidateNotNullOrEmpty ()]
        [string]
        $Path,

        [ValidateNotNullOrEmpty ()]
        [string]
        $Type
    )
    return [GistFile]::new($Name, $Path, $Type)
}

<# 
    .SYNOPSIS
        Creates a new gist that will contain the given collection of files and returns the url
#>
function New-GistWithFiles {
    param (

        [ValidateNotNullOrEmpty ()]
        [string]
        $Description, 

        [Parameter(Mandatory)]
        [GistFile[]]
        $Files,

        [switch]
        $IsPublic=$false # default to false, better save than sorry
    )

    $envVars = @{
        "GITHUB_TOKEN" = $Env:GITHUB_TOKEN;
    }

    foreach ($key in $envVars.Keys) {
        if (-not($envVars[$key])) {
            Write-Debug "Environment variable missing: $key"
            throw [System.InvalidOperationException]::new("Environment variable missing: $key")
        }
    }

    # create the hashtable that will contain all the information of all types
    $payload = @{
        description = $Description;
        files = @{}; # each file is the name of the file + the hashtable of the data to be used
    }

    # switchs are converted to {\"IsPresent\"=>true} in json :/ and the ternary operator might not be in all machines
    if ($IsPublic) {
        $payload["public"] = $true
    } else {
        $payload["public"] = $false
    }

    foreach ($g in $Files) {
        # add the file using its name + the hashtable that is used by GitHub
        $payload["files"].Add($g.Name, $g.ConvertToHashTable())
    }

    $url = "https://api.github.com/gists"
    $payloadJson = $payload | ConvertTo-Json
    Write-Host "Url is $url"
    Write-Host "Payload is $payloadJson"

    $headers = @{
        Accept = "application/vnd.github.v3+json";
        Authorization = ("token {0}" -f $Env:GITHUB_TOKEN);
    } 

    $request = Invoke-Request -Request { Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -Body $payloadJson -ContentType 'application/json' }
    Write-Host $request
    return $request.html_url
}

<#
    .SYNOPSIS
        Puse a repository dispatch stating which branch did trigger it.

    .PARAMETER Org
        The org of the repository to ping.

    .PARAMETER Repository 
        The repository to ping.

    .PARAMETER Branch
        The branch that triggered the event.
#>
function Push-RepositoryDispatch {
    param (

        [ValidateNotNullOrEmpty ()]
        [string]
        $Org, 

        [ValidateNotNullOrEmpty ()]
        [string]
        $Repository,

        [ValidateNotNullOrEmpty ()]
        [string]
        $Branch
    )

    # create the hashtable that will contain all the information of all types
    $payload = @{
        event_type = $Branch;
    }

    $url = "https://api.github.com/repos/$Org/$Repository/dispatches"
    Write-Host $url
    $payloadJson = $payload | ConvertTo-Json

    $headers = @{
        Accept = "application/vnd.github.v3+json";
        Authorization = ("token {0}" -f $Env:GITHUB_TOKEN);
    } 

    $request = Invoke-Request -Request { Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -Body $payloadJson -ContentType 'application/json' }
    Write-Host $request
    Write-Host $request.Content
}

# module exports, any other functions are private and should not be used outside the module.
Export-ModuleMember -Function Set-GitHubStatus
Export-ModuleMember -Function New-GitHubComment
Export-ModuleMember -Function New-GitHubCommentFromFile
Export-ModuleMember -Function New-GitHubSummaryComment 
Export-ModuleMember -Function Test-JobSuccess 
Export-ModuleMember -Function Get-GitHubPRInfo
Export-ModuleMember -Function New-GistWithFiles 
Export-ModuleMember -Function New-GistObjectDefinition 
Export-ModuleMember -Function New-GistWithContent 
Export-ModuleMember -Function Push-RepositoryDispatch 
