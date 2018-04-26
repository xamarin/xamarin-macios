#!/bin/bash -e

if test -z "$ghprbPullId"; then
	echo "Could not find the environment variable ghprbPullId, so it's not possible to fetch the labels for any pull request."
	exit 1
fi

FORCE=
CHECK=
while ! test -z "$1"; do
    case "$1" in
		--force|-f)
			FORCE=1
			shift
			;;
		--check)
			if test -z "$2"; then
				echo "Missing argument to --check"
				exit 1
			fi
			CHECK="$2"
			shift 2
			;;
		--check=*|--check:*)
			CHECK="${1:8}"
			shift
			;;
		*)
			echo "Unknown argument: $1"
			exit 1
			;;
    esac
done

cd "$(dirname "$0")"

TMPFILE=".tmp-labels-$ghprbPullId"
if [[ x"$FORCE" == "x" && ( -f "$TMPFILE" ) ]]; then
	echo "Not downloading labels for pull request #$ghprbPullId because they've already been downloaded."
else
	echo "Downloading labels for pull request #$ghprbPullId..."
	if ! curl --silent "https://api.github.com/repos/xamarin/xamarin-macios/issues/$ghprbPullId/labels" > "$TMPFILE.dl"; then
		echo "Failed to fetch labels for the pull request $ghprbPullId."
		exit 1
	fi
	grep "\"name\":" "$TMPFILE.dl" | sed -e 's/name": \"//' -e 's/.*\"\(.*\)\".*/\1/'> "$TMPFILE" || true
fi

if test -z "$CHECK"; then
	echo "Labels found:"
	sed 's/^/    /' "$TMPFILE"
else
	if grep "^$CHECK$" "$TMPFILE" >/dev/null 2>&1; then
		echo "The pull request $ghprbPullId contains the label $CHECK."
	else
		echo "The pull request $ghprbPullId does not contain the label $CHECK."
		exit 1
	fi
fi
