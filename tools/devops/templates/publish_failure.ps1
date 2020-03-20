# When this runs, a test failure has occurred

Set-Location Env:
Get-ChildItem


$target_url = $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI + "$Env:SYSTEM_TEAMPROJECT/_build/index?buildId=$Env:BUILD_BUILDID&view=ms.vss-test-web.test-result-details"

## don't need context here b/c we are combining all device tests into one post?
#$json_payload = @"{"token": $TOKEN, "hash":$BUILD_REVISION "state": $GH_STATE, "target-url": $TARGET_URL, "description": $DESCRIPTION, "context": "VSTS: device tests $DEVICE_TYPE"}"

# add real device type here
# add description back in
# state: only report failure, so state is always failure but we can also use AGENT_JOBSTATUS to avoid hardcoding values
$json_payload = @"
{
    "state" : "$Env:AGENT_JOBSTATUS",
    "target_url" : "$target_url",
    "description" : "$Env:SYSTEM_JOBNAME", 
    "context" : "$Env:CONTEXT"
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



# Add a GitHub commit comment

# Query job status (always failure) and set result emoji
GH_STATE=failure
DESCRIPTION="Running device tests"
RESULT_EMOJII=
if test -n "$START"; then
	GH_STATE=pending
	DESCRIPTION="Running device tests on $DEVICE_TYPE"
else
	case "$(echo "$AGENT_JOBSTATUS" | tr '[:upper:]' '[:lower:]')" in
		succeeded)
			GH_STATE=success
			DESCRIPTION="Device tests passed on $DEVICE_TYPE"
			RESULT_EMOJII="✅ "
			;;
		failed | canceled | succeededwithissues | *)
			GH_STATE=error
			DESCRIPTION="Device tests completed ($AGENT_JOBSTATUS) on $DEVICE_TYPE"
			RESULT_EMOJII="🔥 "
			;;
	esac
fi



# create a temp file to construct commit message
# add cleanup code
# add message header: 	if [[ $DEVICE_TYPE == *"DDFun"* ]]; then printf "### :construction: Experimental DDFun pipeline\\n" > "$MESSAGE_FILE"
# add the following to the file: $DEVICE_TYPE + emoji + $DESCRIPTION + $VSTS_BUILD_URL + $HTMLREPORT + RESULT_EMOJI
# look for testsummary.md
    # if not found
        # add "🔥 Tests failed catastrophically on $DEVICE_TYPE  (no summary found)\\n"
    # if found
        # cat testsummary.md > messagefile
if test -z "$START"; then
	# When we're done, add a GitHub comment to the commit we're testing
	MESSAGE_FILE=commit-message.txt
	cleanup ()
	{
		rm -f "$MESSAGE_FILE"
	}
	trap cleanup ERR
	trap cleanup EXIT

	HTML_REPORT=""
	if [[ $DEVICE_TYPE == *"DDFun"* ]]; then
		printf "### :construction: Experimental DDFun pipeline\\n" > "$MESSAGE_FILE"
	else
		P=$(cat tmp.p)
		HTML_REPORT=": [Html Report](http://xamarin-storage/${P}/jenkins-results/tests/index.html)"
	fi

	printf "%s%s on [Azure DevOps](%s)($DEVICE_TYPE)%s %s\\n\\n" "$RESULT_EMOJII" "$DESCRIPTION" "$VSTS_BUILD_URL" "$HTML_REPORT" "$RESULT_EMOJII" >> "$MESSAGE_FILE"

	FILE=$PWD/TestSummary.md
	if ! test -f "$FILE"; then
		printf "🔥 Tests failed catastrophically on $DEVICE_TYPE  (no summary found)\\n" >> "$MESSAGE_FILE"
	else
		cat "$FILE" >> "$MESSAGE_FILE"
	fi

	./jenkins/add-commit-comment.sh --token="$TOKEN" --file="$MESSAGE_FILE" "--hash=$BUILD_REVISION"

# construct a json payload and publish it to GH
# url is probably something like:
$comment_url = "https://api.github.com/repos/xamarin/xamarin-macios/commits/$Env:BUILD_REVISION/comments"

# need to print out JSON payload from jenkins to do comparison


    if ! curl -f -v -H "Authorization: token $TOKEN" -H "User-Agent: command line tool" -d "@$JSONFILE" "https://api.github.com/repos/xamarin/xamarin-macios/commits/$HASH/comments" > "$LOGFILE" 2>&1; then
	echo "Failed to add commit message."
	echo "curl output:"
	sed 's/^/    /' "$LOGFILE"
	echo "Json body:"
	sed 's/^/    /' "$JSONFILE"
	exit 1
else
	if test -n "$VERBOSE"; then sed 's/^/    /' "$LOGFILE"; fi
	echo "Successfully added commit message to https://github.com/xamarin/xamarin-macios/commit/$HASH"
fi

fi
#>
