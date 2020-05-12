# When this runs, a test failure has occurred
# --state=<success|pending|error|failure>: The status state.

Get-Location
Set-Location Env:
Get-ChildItem

###
### Construct status
###

$target_url = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI + "$Env:SYSTEM_TEAMPROJECT/_build/index?buildId=$Env:BUILD_BUILDID&view=ms.vss-test-web.test-result-details"

# state: we only report failures
$json_payload = @"
{
    "state" : "failure",
    "target_url" : "$target_url",
    "description" : "$Env:SYSTEM_STAGEDISPLAYNAME",
    "context" : "VSTS: $Env:SYSTEM_JOBNAME"
}
"@

$url = "https://api.github.com/repos/xamarin/xamarin-macios/statuses/$Env:BUILD_REVISION"

Write-Host $json_payload
Write-Host $url

$params = @{
    Uri = $url
    Headers = @{'Authorization' = ("token {0}" -f $Env:GITHUB_TOKEN)}
    Method = 'POST'
    Body = $json_payload
    ContentType = 'application/json'
}

Write-Host $params

$response = Invoke-RestMethod @params

$response | ConvertTo-Json | Write-Host


###
### Construct commit message with test summary
###

$RESULT_EMOJI
If ($Env:AGENT_JOBSTATUS -eq 'Failed')
{
	$RESULT_EMOJI = ":fire: "
} Else {
	$RESULT_EMOJI = ":white_check_mark: " # maybe remove this since we only add commit messages on failure?
}


If ($Env:BUILD_DEFINITIONNAME -like '*DDFun*')
{
    ## do stuff
}
#Else{
#HTML Report jenkins stuff
#}


$HEADER = "### :bangbang: :construction: TESTING Experimental DDFun pipeline"


# SYSTEM_JOBNAME: Xamarin | Monotouch | xUnitBCL | NUnitBCL | mscorlib
# CONTEXT: tvOS | iOS | iOS32
$DESCRIPTION="Device test **$Env:SYSTEM_JOBNAME** $Env:AGENT_JOBSTATUS on **$Env:CONTEXT**"

# BUILD_DEFINITIONNAME: Pipeline name, e.g. "iOS Device Tests [DDFun]"
$json_text = "$RESULT_EMOJI $DESCRIPTION on [Azure DevOps]($target_url) ($Env:BUILD_DEFINITIONNAME) $RESULT_EMOJI"

$file = $Env:SYSTEM_DEFAULTWORKINGDIRECTORY
Get-ChildItem $file | Write-Host

$file = "$file/xamarin-macios/tests"

Write-Host "xamarin-macios tests children"
Get-ChildItem $file | Write-Host

$file = "$file/TestSummary.md"

Write-Host $file
Get-Content $file | Write-Host



# stringbuilder for extra flavor
$msg = [System.Text.StringBuilder]::new()
$msg.AppendLine($HEADER)
$msg.AppendLine()
$msg.AppendLine($json_text)
$msg.AppendLine()

# read each line of the summary file, append it with correct \n at the end
foreach ($line in Get-Content -Path $file)
{
	$msg.AppendLine($line)
}

$message_url = "https://api.github.com/repos/xamarin/xamarin-macios/commits/$Env:BUILD_REVISION/comments"


# create pwsh object to store payload
$payload = @{
	body = $msg.ToString()
}

# convert payload to json
$json_payload = $payload | ConvertTo-json

Write-Host $json_payload

$params = @{
    Uri = $message_url
    Headers = @{'Authorization' = ("token {0}" -f $Env:GITHUB_TOKEN)}
    Method = 'POST'
    Body = $json_payload
    ContentType = 'application/json'
}

Write-Host $params

$response = Invoke-RestMethod @params

$response | ConvertTo-Json | Write-Host
