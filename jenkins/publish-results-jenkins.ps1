Write-Host $pwd.Path
Write-Host $PSScriptRoot
Set-Location -Path $PSScriptRoot
Write-Host $pwd.Path

## don't need context here b/c we are combining all device tests into one post?
#$json_payload = @"{"token": $TOKEN, "hash":$BUILD_REVISION "state": $GH_STATE, "target-url": $TARGET_URL, "description": $DESCRIPTION, "context": "VSTS: device tests $DEVICE_TYPE"}"

# add real device type here
# add description back in
$json_payload = @"
{
    "hash" : $Env:BUILD_REVISION,
    "state" : $Env:GH_STATE,
    "target-url" : $Env:TARGET_URL,
    "description" : "description placeholder", 
    "context" : "VSTS: device tests",
    "vsts-for-debugging" : $Env:VSTS_BUILD_URL
    "GH-token-for-debugging" : $Env:GITHUB_TOKEN
}
"@


$url = "https://api.github.com/repos/xamarin/xamarin-macios/statuses/$BUILD_REVISION"

Write-Host @{'Authorization' = ("token {0}" -f $GITHUB_TOKEN)}
Write-Host $json_payload

$params = @{
    Uri = $url
    Headers = @{'Authorization' = ("token {0}" -f $GITHUB_TOKEN)}
    Method = 'PUT'
    Body = $json_payload
    ContentType = 'application/json'
}

Write-Host $params

$response = Invoke-RestMethod @params

$response | ConvertTo-Json | Write-Host

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