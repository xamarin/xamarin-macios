#!/bin/bash -eu

# Script to add a status to a commit on GitHub.
#
# Arguments (all required):
# --token=<pat>: The GitHub Personal Access Token used to authenticate with GitHub.
# --hash=<hash>: The hash to add the comment to.
# --state=<success|pending|error|failure>: The status state.
# --target-url=<url>: The status url.
# --description=<description>: The status description.
# --context=<context>: The status context.
#

TOKEN=
HASH=
STATE=
TARGET_URL=
DESCRIPTION=
CONTEXT=
VERBOSE=

while ! test -z "${1:-}"; do
	case "$1" in
		--token=*)
			TOKEN="${1:8}"
			shift
			;;
		--token)
			TOKEN="$2"
			shift 2
			;;
		--hash=*)
			HASH="${1:7}"
			shift
			;;
		--hash)
			HASH="$2"
			shift 2
			;;
		--state=*)
			STATE="${1:8}"
			shift
			;;
		--state)
			STATE="$2"
			shift 2
			;;
		--target-url=*)
			TARGET_URL="${1:13}"
			shift
			;;
		--target-url)
			TARGET_URL="$2"
			shift 2
			;;
		--description=*)
			DESCRIPTION="${1:14}"
			shift
			;;
		--description)
			DESCRIPTION="$2"
			shift 2
			;;
		--context=*)
			CONTEXT="${1:10}"
			shift
			;;
		--context)
			CONTEXT="$2"
			shift 2
			;;
		-v | --verbose)
			VERBOSE=1
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

if test -z "$HASH"; then
	echo "The commit hash is required (--hash=<hash>)"
	exit 1
fi

if test -z "$STATE"; then
	echo "The state of the status is required (--state=<STATE>)"
	exit 1
fi

if test -z "$TARGET_URL"; then
	echo "The target url of the status is required (--target-url=<TARGET URL>)"
	exit 1
fi

if test -z "$DESCRIPTION"; then
	echo "The description of the status is required (--description=<DESCRIPTION>)"
	exit 1
fi

if test -z "$CONTEXT"; then
	echo "The context of the status is required (--context=<CONTEXT>)"
	exit 1
fi

JSONFILE=$(mktemp)
LOGFILE=$(mktemp)
cleanup ()
{
	rm -f "$JSONFILE" "$LOGFILE"
}
trap cleanup ERR
trap cleanup EXIT

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
	exit 1
else
	if test -n "$VERBOSE"; then sed 's/^/    /' "$LOGFILE"; fi
	echo "Successfully added status to https://github.com/xamarin/xamarin-macios/commit/$HASH"
fi
