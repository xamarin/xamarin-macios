#!/bin/bash -eu

# Script to add a comment to a commit on GitHub.
#
# Arguments (all required):
# --token=<pat>: The GitHub Personal Access Token used to authenticate with GitHub.
# --file=<path>: The file to add as the comment.
# --hash=<hash>: The hash to add the comment to.
#

TOKEN=
FILE=
HASH=
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
		--file=*)
			FILE="${1:7}"
			shift
			;;
		--file)
			FILE="$2"
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

if test -z "$FILE"; then
	echo "The file to add as a comment is required (--file=<path>)"
	exit 1
elif ! test -f "$FILE"; then
	echo "The file $FILE does not exist"
	exit 1
fi


if test -z "$HASH"; then
	echo "The commit hash is required (--hash=<hash>)"
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

printf '{\n"body": ' > "$JSONFILE"
python -c 'import json,sys; print(json.dumps(sys.stdin.read()))' < "$FILE" >> "$JSONFILE"
printf '}\n' >> "$JSONFILE"

if test -n "$VERBOSE"; then
	echo "JSON file:"
	sed 's/^/    /' "$JSONFILE";
fi

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
