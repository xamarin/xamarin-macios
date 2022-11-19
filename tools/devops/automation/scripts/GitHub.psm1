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
                Write-Host "Error performing request to $($_.Exception.Response.RequestMessage.RequestUri) trying in $seconds seconds"
                Write-Host "Exception was:"
                Write-Host "$($_.Exception)"
                Write-Host "Response was:"
                Write-Host "$($_.Exception.Response)"
                Start-Sleep -Seconds $seconds
            }
        }

    } while ($true)
}

class GitHubStatus {
    [ValidateNotNullOrEmpty ()] [string] $Status
    [ValidateNotNullOrEmpty ()] [string] $Description
    [ValidateNotNullOrEmpty ()] [string] $Context
    [string] $TargetUrl

    GitHubStatus(
        $status,
        $description,
        $context
    ) {
        if (-not $("error", "failure", "pending", "success").Contains($status)) {
            throw [System.ArgumentOutOfRangeException]::new("status")
        }
        $this.Status = $status
        $this.Description = $description
        $this.Context = $context
        $this.TargetUrl = $null
    }

    GitHubStatus(
        $status,
        $description,
        $context,
        $targetUrl
    ) {
        if (-not $("error", "failure", "pending", "success").Contains($status)) {
            throw [System.ArgumentOutOfRangeException]::new("status")
        }
        $this.Status = $status
        $this.Description = $description
        $this.Context = $context
        $this.TargetUrl = $targetUrl
    }

}

class GitHubStatuses {
    [ValidateNotNullOrEmpty ()][string] $Org
    [ValidateNotNullOrEmpty ()][string] $Repo
    [ValidateNotNullOrEmpty ()][string] $Token

    GitHubStatuses (
        $githubOrg,
        $githubRepo,
        $githubToken
    ) {
        $this.Org = $githubOrg
        $this.Repo = $githubRepo
        $this.Token = $githubToken
    }

    [string] GetStatusUrl() {
        if ($Env:BUILD_REASON -eq "PullRequest") {
            # the env var is only provided for PR not for builds.
            $url = "https://api.github.com/repos/$($this.Org)/$($this.Repo)/statuses/$Env:SYSTEM_PULLREQUEST_SOURCECOMMITID"
        } else {
            $url = "https://api.github.com/repos/$($this.Org)/$($this.Repo)/statuses/$Env:BUILD_SOURCEVERSION"
        }
        return $url
    }

    [string] GetStatusPrefix() {
        if ($Env:BUILD_REASON -eq "PullRequest") {
            return "[PR]"
        } else {
            return "[CI]"
        }
    }

    [object] SetStatus($status) {
        return $this.SetStatus(
            $status.Status,
            $status.Description,
            $status.Context,
            $status.TargetUrl)
    }

    [object] SetStatus($status, $description, $context, $targetUrl) {
        # set a prefix to be more clear
        $context = "$($this.GetStatusPrefix()) $context"
        $headers = @{
            Authorization = ("token {0}" -f $this.Token)
        }

        $url = $this.GetStatusUrl()

        # Check if the status was already set, if it was we will override yet print a message for the user to know this action was done.
        $presentStatuses = Invoke-Request -Request { Invoke-RestMethod -Uri $url -Headers $headers -Method "GET" -ContentType 'application/json' }

        # try to find the status with the same context and make a decision, this is not a dict but an array :/ 
        foreach ($s in $presentStatuses) {
            # we found a status from a previous build that was a success, we do not want to step on it
            if (($s.context -eq $context) -and ($s.state -eq "success")) {
                Write-Debug "WARNING: Found status for $Context because it was already set as a success, overriding result."
            }
        }

        # use the GitHub API to set the status for the given commit
        $detailsUrl = ""
        if ($targetUrl) {
            $detailsUrl = $targetUrl
        } else {
            $detailsUrl = Get-TargetUrl
        }

        $payload= @{
            state = $status
            target_url = $detailsUrl
            description = $description
            context = $context
        }

        return Invoke-Request -Request { Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -Body ($payload | ConvertTo-json) -ContentType 'application/json' }
    }

    [object] SetStatus($status, $description, $context) {
        return $this.SetStatus($status, $description, $context, $null)
    }
}

<#
    .SYNOPSIS
        Creates a new GitHubComments object from that can be used to create comments for the build.
#>
function New-GitHubStatusesObject {
    param (

        [ValidateNotNullOrEmpty ()]
        [string]
        $Org,

        [ValidateNotNullOrEmpty ()]
        [string]
        $Repo,

        [ValidateNotNullOrEmpty ()]
        [string]
        $Token
    )
    return [GitHubStatuses]::new($Org, $Repo, $Token)
}

function New-GitHubStatusesObjectFromUrl {
    param (

        [ValidateNotNullOrEmpty ()]
        [string]
        $Url,

        [ValidateNotNullOrEmpty ()]
        [string]
        $Token
    )

    $orgRepo = $Url.Replace("https://github.com/", "")
    $org, $repo = $orgRepo -split "/",2

    return [GitHubStatuses]::new($org, $repo, $Token)
}


class GitHubComment {
    [string] $Id
    [string] $Author
    [string] $Body
    [bool] $IsMinimized

    GitHubComment(
        [string] $id,
        [string] $author,
        [string] $body,
        [bool] $isMinimized
    ) {
        $this.Id = $id
        $this.Author = $author
        $this.Body = $body
        $this.IsMinimized = $isMinimized
    }
}

class GitHubComments {
    [ValidateNotNullOrEmpty ()][string] $Org
    [ValidateNotNullOrEmpty ()][string] $Repo
    [ValidateNotNullOrEmpty ()][string] $Token
    [string] $Hash
    hidden static [string] $GitHubGraphQLEndpoint = "https://api.github.com/graphql"

    GitHubComments (
        $githubOrg,
        $githubRepo,
        $githubToken
    ) {
        $this.Org = $githubOrg
        $this.Repo = $githubRepo
        $this.Token = $githubToken
        $this.Hash = $null
    }

    GitHubComments (
        $githubOrg,
        $githubRepo,
        $githubToken,
        $hash
    ) {
        $this.Org = $githubOrg
        $this.Repo = $githubRepo
        $this.Token = $githubToken
        $this.Hash = $hash
    }

    static [bool] IsPR() {
        return $Env:BUILD_REASON -eq "PullRequest"
    }

    static [string] GetPRID() {
        $buildSourceBranch = $Env:BUILD_SOURCEBRANCH
        $changeId = $buildSourceBranch.Replace("refs/pull/", "").Replace("/merge", "")
        return $changeId 
    }

    [void] WriteCommentHeader(
        [object] $stringBuilder,
        [string] $commentTitle,
        [string] $commentEmoji
    ) {
        # Don't write a header if none was provided
        if ($commentTitle.Length -eq 0) {
            return
        }

        if ([string]::IsNullOrEmpty($Env:PR_ID)) {
            $prefix = "[CI Build]"
        } else {
            $prefix = "[PR Build]"
        }

        $stringBuilder.AppendLine("# $commentEmoji $prefix $commentTitle $commentEmoji")
        $stringBuilder.AppendLine()
    }

    [void] WriteCommentFooter(
        [object] $stringBuilder
    ) {
        $targetUrl = Get-TargetUrl
        $stringBuilder.AppendLine("[Pipeline]($targetUrl) on Agent $Env:TESTS_BOT") # Env:TESTS_BOT is added by the pipeline as a variable coming from the execute tests job
        $hashUrl = $null
        $hashSource = $null
        if ([GitHubComments]::IsPR()) {
            $changeId = [GitHubComments]::GetPRID()
            $hashUrl = "https://github.com/$($this.Org)/$($this.Repo)/pull/$changeId/commits/$($this.Hash)"
            $hashSource = " [PR build]"
        } else {
            $hashUrl= "https://github.com/$($this.Org)/$($this.Repo)/commit/$($this.Hash)"
            $hashSource = " [CI build]"
        }
        $ciComment = "[comment]: <> (This is a comment added by Azure DevOps)"
        $stringBuilder.AppendLine("Hash: [$($this.Hash)]($hashUrl) $hashSource")
        $stringBuilder.AppendLine("")
        $stringBuilder.AppendLine($ciComment)
    }

    [string] GetCommentUrl() {
        # if the build was due to PR, we want to write the comment in the PR rather than in the commit 
        if ([GitHubComments]::IsPR()) {
            $changeId = [GitHubComments]::GetPRID()
            $url = "https://api.github.com/repos/$($this.Org)/$($this.Repo)/issues/$changeId/comments"
        } else {
            if ($this.Hash) {
                $url = "https://api.github.com/repos/$($this.Org)/$($this.Repo)/commits/$($this.Hash)/comments"
            } else {
                $url = "https://api.github.com/repos/$($this.Org)/$($this.Repo)/commits/$Env:BUILD_SOURCEVERSION/comments"
            }
        }
        return $url
    }

    [string] GetPayload($stringBuilder) {
        # github has a max size for the comments to be added in a PR, it can be the case that because we failed so much, that we
        # cannot add the full message, in that case, we add part of it, then a link to a gist with the content.
        $maxLength = 32768
        $body = $stringBuilder.ToString()
        if ($body.Length -ge $maxLength) {
            # create a gist with the contents, next, add substring of the message - the length of the info about the gist so that users
            # can click, set that as the body
            $gist =  New-GistWithContent -Description "Build results" -FileName "TestResult.md" -GistContent $body -FileType "md"
            $linkMessage = "The message from CI is too large for the GitHub comments. You can find the full results [here]($gist)."
            $messageLength = $maxLength - ($linkMessage.Length + 2) # +2 is to add a nice space
            $body = $body.Substring(0, $messageLength);
            $body = $body + "\n\n" + $linkMessage
        }
        return $body
    }

    hidden [object] NewComment($stringBuilder) {
        $payload = @{
            body = $this.GetPayload($stringBuilder)
        }

        $headers = @{
            Authorization = ("token {0}" -f $this.Token)
        }

        $url = $this.GetCommentUrl()
        $request = Invoke-Request -Request { Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -Body ($payload | ConvertTo-Json) -ContentType 'application/json' }
        return $request
    }

    [object] NewCommentFromObject(
        [string] $commentTitle,
        [string] $commentEmoji,
        [object] $commentObject
    ) {
        # build the message, which will be sent to github, users can use markdown
        $msg = [System.Text.StringBuilder]::new()

        # header
        $this.WriteCommentHeader($msg, $commentTitle, $commentEmoji)

        # content
        $commentObject.WriteComment($msg)
        $msg.AppendLine()

        # footer
        $this.WriteCommentFooter($msg)

        return $this.NewComment($msg)
    }

    [object] NewCommentFromFile(
        [string] $commentTitle,
        [string] $commentEmoji,
        [string] $filePath
    ) {
        # build the message, which will be sent to github, users can use markdown
        $msg = [System.Text.StringBuilder]::new()

        # header
        $this.WriteCommentHeader($msg, $commentTitle, $commentEmoji)

        if (-not (Test-Path $filePath -PathType Leaf)) {
            throw [System.IO.FileNotFoundException]::new($filePath)
        }

        # content
        foreach ($line in Get-Content -Path $filePath)
        {
            $msg.AppendLine($line)
        }
        $msg.AppendLine()

        # footer
        $this.WriteCommentFooter($msg)

        return $this.NewComment($msg)
    }

    [object] NewCommentFromMessage(
        [string] $commentTitle,
        [string] $commentEmoji,
        [string] $content
    ) {
        $msg = [System.Text.StringBuilder]::new()

        # header
        $this.WriteCommentHeader($msg, $commentTitle, $commentEmoji)

        # content
        $msg.AppendLine($content)
        $msg.AppendLine()

        # footer
        $this.WriteCommentFooter($msg)

        return $this.NewComment($msg)
    }

    [object] GetCommentsForPR ($prId) {
        # build the query, create the json and perform a rest request againt the grapichQl api
        $url = [GitHubComments]::GitHubGraphQLEndpoint
        $headers = @{
            Authorization = ("Bearer {0}" -f $this.Token)
        }

        $query = @"
query {
    repository(owner:"$($this.Org)", name:"$($this.Repo)"){
        pullRequest(number: $prID) {
            comments(last: 100) {
                edges {
                    node {
                        id
                        isMinimized
                        body
                        author {
                            login
                        }
                    }
                }
            }
        }
    }
}
"@
        $payload = @{
            query=$query
        }
        $body = ConvertTo-Json $payload
        $response = Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -Body $body
        # loop over the result and remove all the extra noise we are not interested in
        $comments = [System.Collections.ArrayList]@()
        foreach ($edge in $response.data.repository.pullRequest.comments.edges) {
            $commentId = $edge.node.id
            $isMinimized = $edge.node.isMinimized
            $author = $edge.node.author.login
            $body = $edge.node.body
            $comments.Add([GitHubComment]::new($commentId, $author, $body, $isMinimized))
        }
        # at this point, we have the comments for the PR, but not the comments of the commits of the PR, yes, confusing. The github UI
        # contains 2 sets of comments:
        # 1. Comments on a PR (which is an issue really)
        # 2. Comments on the commits that are part of the PR
        #
        # We are missing those in 2, we can get those with a second query

        $query = @"
query{
    repository(owner:"$($this.Org)", name:"$($this.Repo)") {
        pullRequest(number: $prID){
            commits(last:100) {
                edges {
                    node {
                        commit {
                            ... on Commit {
                                oid
                                message
                            } 
                            comments (last:100) {
                                edges {
                                    node {
                                        id
                                        body
                                        author {
                                            login
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
"@
        $payload = @{
            query=$query
        }
        $body = ConvertTo-Json $payload
        $response= Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -Body $body
        foreach ($edge in $response.data.repository.pullRequest.commits.edges) {
            # at this point a node is a commit, which has the following:
            # commit
            #   oid
            #   message
            #   comments
            #       edges
            #
            # we are interested in looping for the comments
            foreach ($commentEdge in $edge.node.commit.comments.edges) {
                $commentId = $commentEdge.node.id
                $isMinimized = $commentEdge.node.isMinimized
                $author = $commentEdge.node.author.login
                $body = $commentEdge.node.body
                $comments.Add([GitHubComment]::new($commentId, $author, $body, $isMinimized))
            }
        }
        return $comments
    }

    [void] MinimizeComments($comments) {
        $headers = @{
            Authorization = ("Bearer {0}" -f $this.Token)
        }
        # we cannot do a mutation with all the comments :/ but we can loop and do it
        foreach($c in $comments) {

        $mutation =@"
mutation {
    __typename
    minimizeComment(
        input: {
            subjectId: "$($c.Id)",
            clientMutationId: "xamarin-macios-ci"
            classifier: OUTDATED
        }
    ) {
        clientMutationId
    }
}
"@
            $payload = @{
                query=$mutation
            }
            $body = ConvertTo-Json $payload
            $url = [GitHubComments]::GitHubGraphQLEndpoint
            $response= Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -Body $body
        } # foreach
    }
}

<# 
    .SYNOPSIS
        Creates a new GitHubComments object from that can be used to create comments for the build.
#>
function New-GitHubCommentsObject {
    param (

        [ValidateNotNullOrEmpty ()]
        [string]
        $Org,

        [ValidateNotNullOrEmpty ()]
        [string]
        $Repo,

        [ValidateNotNullOrEmpty ()]
        [string]
        $Token,

        [string]
        $Hash

    )
    Write-Debug "New-GitHubCommentsObject ('$Org', '$Repo', '$Token', '$Hash')"
    if ($Hash) {
        return [GitHubComments]::new($Org, $Repo, $Token, $Hash)
    } else {
        return [GitHubComments]::new($Org, $Repo, $Token)
    }
}

<# 
    .SYNOPSIS
        Creates a new GitHubComments object from a repo url so that can be used to create comments for the build.
#>
function New-GitHubCommentsObjectFromUrl {
    param (

        [ValidateNotNullOrEmpty ()]
        [string]
        $Url,

        [ValidateNotNullOrEmpty ()]
        [string]
        $Token,

        [string]
        $Hash

    )
    Write-Debug "New-GitHubCommentsObjectFromUrl ('$Url', '$Token', '$Hash')"
    $orgRepo = $Url.Replace("https://github.com/", "")
    $org, $repo = $orgRepo -split "/",2
    return New-GitHubCommentsObject -Org $org -Repo $repo -Token $Token -Hash $Hash
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
        * BUILD_SOURCEVERSION: The revision of the current build. Needed to know the commit whose status to change.
        * GITHUB_TOKEN: OAuth or PAT token to interact with the GitHub API.
#>
function New-GitHubComment {
    param
    (
        [Parameter(Mandatory)]
        [String]
        $Header,
        
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
        "BUILD_SOURCEVERSION" = $Env:BUILD_SOURCEVERSION;
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
    if ($Description) { # only if description is not null or empty
        $msg.AppendLine($Description)
    }
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
        $url = "https://api.github.com/repos/xamarin/xamarin-macios/commits/$Env:BUILD_SOURCEVERSION/comments"
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
    Write-Debug $request
    return $request
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

    $headers = @{
        Authorization = ("token {0}" -f $Env:GITHUB_TOKEN);
    }

    $request = Invoke-Request -Request { Invoke-RestMethod -Uri $url -Method "GET" -ContentType 'application/json' -Headers $headers }
    Write-Debug $request
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
    Write-Debug "Url is $url"
    Write-Debug "Payload is $payloadJson"

    $headers = @{
        Accept = "application/vnd.github.v3+json";
        Authorization = ("token {0}" -f $Env:GITHUB_TOKEN);
    } 

    $request = Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -Body $payloadJson -ContentType 'application/json'
    Write-Debug $request
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
    Write-Debug "Url is $url"
    Write-Debug "Payload is $payloadJson"

    $headers = @{
        Accept = "application/vnd.github.v3+json";
        Authorization = ("token {0}" -f $Env:GITHUB_TOKEN);
    } 

    $request = Invoke-Request -Request { Invoke-RestMethod -Uri $url -Headers $headers -Method "POST" -Body $payloadJson -ContentType 'application/json' }
    Write-Debug $request
    return $request.html_url
}

# This function processes markdown, and replaces:
#     1. "[vsdrops](" with "[vsdrops](https://link/to/vsdrops/".
#     2. "[gist](file)" with "[gist](url)" after uploading "file" to a gist.
# It also takes a root directory parameter, which is where files to gist should be searched for
function Convert-Markdown {
    param (
        [string]
        $RootDirectory,

        [string]
        $InputContents,

        [string]
        $VSDropsPrefix
    )

    $InputContents = $InputContents.Replace("[vsdrops](", "[vsdrops](" + $VSDropsPrefix)

    $startIndex = $InputContents.IndexOf("[gist](", $index)
    while ($startIndex -gt 0) {
        $endIndex =$InputContents.IndexOf(")", $startIndex + 7)
        if ($endIndex -gt $startIndex) {
            $fileToGist = $InputContents.Substring($startIndex + 7, $endIndex - $startIndex - 7)
            $fullPath = Join-Path -Path $RootDirectory -ChildPath $fileToGist
            if (Test-Path $fullPath -PathType leaf) {
                $gistContent = Get-Content -Path $fullPath -Raw
                if ($gistContent.Length -eq 0) {
                        $gistText = "[empty gist]"
                } else {
                    # github only accepts filename without path components
                    $filenameForGist = [System.Linq.Enumerable]::Last($fileToGist.Split("/", [StringSplitOptions]::RemoveEmptyEntries))
                    $obj = New-GistObjectDefinition -Name $filenameForGist -Path $fullPath -Type "markdown"
                    $filesToGist = ($obj)
                    try {
                        $gistUrl = New-GistWithFiles $fileToGist $filesToGist
                        $gistText = "[gist](" + $gistUrl + ")"
                    } catch {
                        Write-Debug "Unable to create gist: $_"
                        $gistText = "Unable to create gist: $($_.Exception.Message)"
                    }
                }
            } else {
                $gistText = "(could not create gist: file '$fullPath' does not exist)"
            }
            $InputContents = $InputContents.Substring(0, $startIndex) + $gistText + $InputContents.Substring($endIndex + 1)
        } else {
            break
        }

        $startIndex = $InputContents.IndexOf("[gist](", $endIndex)
    }

    return $InputContents
}

# module exports, any other functions are private and should not be used outside the module.
Export-ModuleMember -Function New-GitHubComment
Export-ModuleMember -Function Get-GitHubPRInfo
Export-ModuleMember -Function New-GistWithFiles 
Export-ModuleMember -Function New-GistObjectDefinition 
Export-ModuleMember -Function New-GistWithContent 
Export-ModuleMember -Function Convert-Markdown

# new future API that uses objects.
Export-ModuleMember -Function New-GitHubCommentsObject
Export-ModuleMember -Function New-GitHubCommentsObjectFromUrl
Export-ModuleMember -Function New-GitHubStatusesObject
Export-ModuleMember -Function New-GitHubStatusesObjectFromUrl
