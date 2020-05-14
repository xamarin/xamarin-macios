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
    "description" : "$Env:AGENT_JOBSTATUS",
    "context" : "VSTS: $Env:SYSTEM_JOBNAME $Env:SYSTEM_STAGEDISPLAYNAME"
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

try {
    $response = Invoke-RestMethod @params
    $response | ConvertTo-Json | Write-Host
} catch {
    Write-Host "Error posting GH status: $_"
}

###
### Construct commit message with test summary
###

$RESULT_EMOJI = ":fire: " # we only run when there's a failure

If ($Env:BUILD_DEFINITIONNAME -like '*DDFun*')
{
    ## do stuff
}

$HEADER = "### :bangbang: :construction: TESTING Experimental DDFun pipeline"

# SYSTEM_JOBNAME: Xamarin | Monotouch | xUnitBCL | NUnitBCL | mscorlib
# CONTEXT: tvOS | iOS | iOS32
$DESCRIPTION="Device test **$Env:SYSTEM_JOBNAME** $Env:AGENT_JOBSTATUS on **$Env:CONTEXT**"

# BUILD_DEFINITIONNAME: Pipeline name, e.g. "iOS Device Tests [DDFun]"
$json_text = "$RESULT_EMOJI $DESCRIPTION on [Azure DevOps]($target_url) ($Env:BUILD_DEFINITIONNAME) $RESULT_EMOJI"

# Grab test results from default working directory
# Cannot access the published pipeline artifact because it is not always available when this script runs :(
$file = "$Env:SYSTEM_DEFAULTWORKINGDIRECTORY/xamarin-macios/tests"

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

try {
    $response = Invoke-RestMethod @params
    $response | ConvertTo-Json | Write-Host
} catch {
    Write-Host "Error posting GH commit message: $_"
}
