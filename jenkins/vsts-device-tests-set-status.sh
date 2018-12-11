#!/bin/bash -eux

cd "$(dirname "${BASH_SOURCE[0]}")/.."

TOKEN=
START=
while ! test -z "$1"; do
	case "$1" in
		--token=*)
			TOKEN="${1:8}"
			shift
			;;
		--token)
			TOKEN="$2"
			shift 2
			;;
		--start)
			START=1
			shift
			;;
		*)
			echo "Unknown argument: $1"
			exit 1
			;;
    esac
done

if test -z "$TOKEN"; then
	echo "The GitHub token is required (--token=<TOKEN>)"
	exit 1
fi

P=$(cat tmp.p)

VSTS_BUILD_URL="${SYSTEM_TEAMFOUNDATIONCOLLECTIONURI}${SYSTEM_TEAMPROJECT}/_build/index?buildId=${BUILD_BUILDID}"

# Add a GitHub status to the commit we're testing
GH_STATE=failure
DESCRIPTION="Running device tests"
RESULT_EMOJII=
if test -n "$START"; then
	GH_STATE=pending
	DESCRIPTION="Running device tests"
else
	case "$(echo "$AGENT_JOBSTATUS" | tr '[:upper:]' '[:lower:]')" in
		succeeded)
			GH_STATE=success
			DESCRIPTION="Device tests passed"
			RESULT_EMOJII="✅ "
			;;
		failed | canceled | succeededwithissues | *)
			GH_STATE=error
			DESCRIPTION="Device tests completed ($AGENT_JOBSTATUS)"
			RESULT_EMOJII="🔥 "
			;;
	esac
fi
./jenkins/add-commit-status.sh --token="$TOKEN" --hash="$BUILD_REVISION" --state="$GH_STATE" --target-url="$VSTS_BUILD_URL" --description="$DESCRIPTION" --context="VSTS: device tests"

if test -z "$START"; then
	# When we're done, add a GitHub comment to the commit we're testing
	MESSAGE_FILE=commit-message.txt
	cleanup ()
	{
		rm -f "$MESSAGE_FILE"
	}
	trap cleanup ERR
	trap cleanup EXIT

	printf "%s%s on [Azure DevOps](%s): [Html Report](http://xamarin-storage/%s/jenkins-results/tests/index.html) %s\\n\\n" "$RESULT_EMOJII" "$DESCRIPTION" "$VSTS_BUILD_URL" "$P" "$RESULT_EMOJII" > "$MESSAGE_FILE"

	FILE=$PWD/tests/TestSummary.md
	if ! test -f "$FILE"; then
		printf "🔥 Tests failed catastrophically (no summary found)\\n" >> "$MESSAGE_FILE"
	else
		cat "$FILE" >> "$MESSAGE_FILE"
	fi

	./jenkins/add-commit-comment.sh --token="$TOKEN" --file="$MESSAGE_FILE" "--hash=$BUILD_REVISION"
fi
