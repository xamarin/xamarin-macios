# debugging code: print out all environment variables, including those passed in via yml
Get-Location
Set-Location Env:
Get-ChildItem


$testsummary_location = $Env:SYSTEM_DEFAULTWORKINGDIRECTORY
Get-ChildItem $testsummary_location

$testsummary_location = $testsummary_location + "/TestSummary.md"
Write-Host $testsummary_location
Get-Content $testsummary_location

# get combined status:
# success only if every status is success
# otherwise failure

# url to query for combined status
$combined_status_url = "https://api.github.com/repos/xamarin/xamarin-macios/commits/$Env:BUILD_REVISION/status"

$params = @{
    Uri = $combined_status_url
    Headers = @{'Authorization' = ("token {0}" -f $Env:GITHUB_TOKEN)}
    Method = 'GET'
    ContentType = 'application/json'
}

#
$response = Invoke-RestMethod @params

Write-Host "Raw response: " + $response

Write-Host "Response.state: " + $response.state
$state = $response.state


# post status to github
$target_url = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI + "$Env:SYSTEM_TEAMPROJECT/_build/index?buildId=$Env:BUILD_BUILDID&view=ms.vss-test-web.test-result-details"

## don't need context here b/c we are combining all device tests into one post?
#$json_payload = @"{"token": $TOKEN, "hash":$BUILD_REVISION "state": $GH_STATE, "target-url": $TARGET_URL, "description": $DESCRIPTION, "context": "VSTS: device tests $DEVICE_TYPE"}"

# add real device type here
# add description back in
$json_payload = @"
{
    "state" : "$state",
    "target_url" : "$target_url",
    "description" : "description placeholder",
    "context" : "VSTS: AGGREGATE device tests"
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

$response | Write-Host


###
### Construct commit message w/ aggregate test summary
###

If ($Env:BUILD_DEFINITIONNAME -like '*DDFun*')
{
	# do stuff
}


# BUILD_DEFINITIONNAME: Pipeline name, e.g. "iOS Device Tests [DDFun]"
$json_text = "### :boom: :construction: TESTING Experimental DDFun pipeline: Device test aggregate results: on [Azure DevOps]($target_url)"

$dir = $Env:SYSTEM_DEFAULTWORKINGDIRECTORY
$dir = $dir + "/Summaries/*"
# Get all test summary files
$files = Get-ChildItem -Path $dir -Include TestSummary.md

# stringbuilder for extra flavor
$msg = [System.Text.StringBuilder]::new()
$msg.AppendLine($json_text)
$msg.AppendLine()

foreach ($file in $files)
{
	Write-Host $file
	Write-Host Get-Content $file

	$msg.AppendLine("blah title from filename")
	$msg.AppendLine()

	# read each line of the summary file, append it with correct \n at the end
	foreach ($line in Get-Content -Path $file)
	{
		$msg.AppendLine($line)
	}

	# new line to separate file contents
	$msg.AppendLine()
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
