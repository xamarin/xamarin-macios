# TODO: set up so that we only log debugging
# debugging code: print out all environment variables, including those passed in via yml
Get-Location
Set-Location Env:
Get-ChildItem

###
### Construct commit status with combined test results
###

###
### Get combined status:
###     Success only if every status is success, otherwise failure
###     If something goes wrong when we Invoke-RestMethod, status is set to error
###

# url to query for combined status
$combined_status_url = "https://api.github.com/repos/xamarin/xamarin-macios/commits/$Env:BUILD_REVISION/status"

$params = @{
    Uri = $combined_status_url
    Headers = @{'Authorization' = ("token {0}" -f $Env:GITHUB_TOKEN)}
    Method = 'GET'
    ContentType = 'application/json'
}

$state = "error"
try {
    $response = Invoke-RestMethod @params
    $state = $response.state
} catch {
    Write-Host "Error querying for GH status: $_"
}

###
### Post commit status to GitHub
###

$target_url = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI + "$Env:SYSTEM_TEAMPROJECT/_build/index?buildId=$Env:BUILD_BUILDID&view=ms.vss-test-web.test-result-details"

$json_payload = @"
{
    "state" : "$state",
    "target_url" : "$target_url",
    "description" : "$state",
    "context" : "VSTS: All device tests"
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
    $response | Write-Host
} catch {
    Write-Host "Error posting GH status: $_"
}

###
### Construct commit message w/ aggregate test summary
###

If ($Env:BUILD_DEFINITIONNAME -like '*DDFun*')
{
	# do stuff
}

$dir = "$Env:PIPELINE_WORKSPACE/Summaries/TestSummary-*/TestSummary.md"

Write-Host $dir
Get-ChildItem $dir | Write-Host

# Test summary filepath follows the format:
# /Users/xamarinqa/azdo/_work/15/Summaries/TestSummary-DEVICE-TESTNAME/TestSummary.md
# /Users/xamarinqa/azdo/_work/15/Summaries/TestSummary-tvOS-monotouch-DDFun/TestSummary.md
# /Users/xamarinqa/azdo/_work/15/Summaries/TestSummary-tvOS-xamarin-DDFun/TestSummary.md
# Get all test summary files
$files = Get-ChildItem -Path $dir

# BUILD_DEFINITIONNAME: Pipeline name, e.g. "iOS Device Tests [DDFun]"
$header = "### :boom: :construction: TESTING Experimental DDFun pipeline: Device test aggregate results: on [Azure DevOps]($target_url)"

# stringbuilder for extra flavor
$msg = [System.Text.StringBuilder]::new()
$msg.AppendLine($header)
$msg.AppendLine()

$prefix="$Env:PIPELINE_WORKSPACE/Summaries/TestSummary-"
$suffix="/TestSummary.md"

foreach ($file in $files)
{
    # grab the test name + device string from filepath
    $info = $file.FullName.Substring($prefix.Length, $file.FullName.Length - $suffix.Length - $prefix.Length)

    # switch to keep track of when we are on the first line of a summary
    $first_line = 1

	# read each line of the summary file, append it with correct \n at the end
	foreach ($line in Get-Content -Path $file)
	{
        # if reading the first line, append test + device info to the header
        # $msg will look like: "Test results: tvOS-monotouch-DDFun"
        if ($first_line)
        {
            $msg.Append($line + ": ")
            $msg.AppendLine($info)
            $first_line = 0
        }
        else {
            $msg.AppendLine($line)
        }
	}

	# new line to separate file contents
	$msg.AppendLine()
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
