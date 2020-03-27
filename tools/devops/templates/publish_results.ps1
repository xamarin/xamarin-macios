# debugging code: print out all environment variables, including those passed in via yml
Set-Location Env:
Get-ChildItem

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

Write-Host $response
$response | ConvertTo-Json | Write-Host
Write-Host "^ ConvertToJson below is ConvertFromJson `n"

$response | ConvertTo-Json | ConvertFrom-Json | Write-Host

#$status = $response | ConvertTo-Json | ConvertFrom-Json | Select-Object state

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

$response | ConvertFrom-Json | Write-Host


# https://api.github.com/xamarin/xamarin-macios/commits/eea6fd1f27ba9a0ac4fa09c8e57fc87d612b6340/status

#GET /projects/:id/repository/commits/:sha/refs


#https://api.github.com/repos/xamarin/xamarin-macios/statuses/$Env:BUILD_REVISION



# -Uri $url
# -Method Post
# -Body $json_payload
#$response = Invoke-RestMethod -Uri $url -ContentType application/json  -Method Post -Body @json_payload
<# 
(
	printf '{\n'
	printf "\t\"state\": \"%s\",\n" "$STATE"
	printf "\t\"target_url\": \"%s\",\n" "$TARGET_URL"
	printf "\t\"description\": %s,\n" "$(echo -n "$DESCRIPTION" | python -c 'import json,sys; print(json.dumps(sys.stdin.read()))')"
	printf "\t\"context\": \"%s\"\n" "$CONTEXT"
	printf "}\n"
) > "$JSONFILE"

if test -n "$VERBOSE"; then
	echo "JSON file:"
	sed 's/^/    /' "$JSONFILE";
fi

if ! curl -f -v -H "Authorization: token $TOKEN" -H "User-Agent: command line tool" -d "@$JSONFILE" "https://api.github.com/repos/xamarin/xamarin-macios/statuses/$HASH" > "$LOGFILE" 2>&1; then
	echo "Failed to add status."
	echo "curl output:"
	sed 's/^/    /' "$LOGFILE"
	echo "Json body:"
	sed 's/^/    /' "$JSONFILE"
	exit 1 #>

# 	printf "\t\"state\": \"%s\",\n" "$STATE"
#	printf "\t\"target_url\": \"%s\",\n" "$TARGET_URL"
#	printf "\t\"description\": %s,\n" "$(echo -n "$DESCRIPTION" | python -c 'import json,sys; print(json.dumps(sys.stdin.read()))')"
#	printf "\t\"context\": \"%s\"\n" "$CONTEXT"
# ./jenkins/add-commit-status.sh --token="$TOKEN" --hash="$BUILD_REVISION" --state="$GH_STATE" --target-url="$VSTS_BUILD_URL" --description="$DESCRIPTION" --context="VSTS: device tests ($DEVICE_TYPE)"