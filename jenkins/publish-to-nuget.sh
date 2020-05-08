#!/bin/bash -e

# Script to publish nugets.
#
# Arguments (all required):
# --apikey=<pat>: The api key to authenticate with the nuget server.
# --source=<url>: The nuget server
#

INITIAL_CD=$(pwd)

cd "$(dirname "${BASH_SOURCE[0]}")/.."
WORKSPACE=$(pwd)

CONFIG_FILE=.nuget.config.tmp

report_error ()
{
	# remove config
	rm -f $CONFIG_FILE

	# Show error
	printf "🔥 [Failed to publish to nuget](%s/console) 🔥\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
}
trap report_error ERR

realpath() {
    [[ $1 = /* ]] && echo "$1" || echo "$INITIAL_CD/${1#./}"
}

APIKEY=
SOURCE=
VERBOSITY=()
NUGETS=()

while ! test -z "${1:-}"; do
	case "$1" in
		--apikey=*)
			APIKEY="${1:9}"
			shift
			;;
		--apikey)
			APIKEY="$2"
			shift 2
			;;
		--source=*)
			SOURCE="${1:9}"
			shift
			;;
		--source)
			SOURCE="$2"
			shift 2
			;;
		-v | --verbose)
			VERBOSITY=("-Verbosity" "detailed")
			set -x
			shift
			;;
		-*)
			echo "Unknown argument: $1"
			exit 1
			;;
		*)
			NUGETS+=("$1")
			shift
			;;
    esac
done

if test -z "$APIKEY"; then
	echo "The API key is required (--apikey=<apikey>)"
	exit 1
fi

if test -z "$SOURCE"; then
	echo "The source is required (--source=<source>)"
	exit 1
fi

if [[ "x${#NUGETS[@]}" == "x0" ]]; then
	echo "No nupkgs provided."
	exit 1
fi

# create empty config for auth
echo '<?xml version="1.0" encoding="utf-8"?><configuration></configuration>' > $CONFIG_FILE

# add the feed
nuget sources add -name FEEDME -source "$SOURCE" -username WHATEVER -password "$APIKEY" -config $CONFIG_FILE

# push
for nuget in "${NUGETS[@]}"; do
	nuget="$(realpath "$nuget")"
	nuget push "$nuget" -Source FEEDME -ApiKey WHATEVER -NonInteractive "${VERBOSITY[@]}" -ConfigFile $CONFIG_FILE
done

# remove config
rm -f $CONFIG_FILE
