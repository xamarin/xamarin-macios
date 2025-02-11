#!/bin/bash -eu

set -o pipefail
IFS=$'\n\t '

OUTPUT=Make.config.inc
OUTPUT_FILE=Make.config.inc.tmp

rm -f "$OUTPUT_FILE" "$OUTPUT"

LANG=C
export LANG

# Compute commit distances
for platform in $ALL_DOTNET_PLATFORMS; do
	PLATFORM=$(echo "$platform" | tr '[:lower:]' '[:upper:]')
	COMMIT=$(git blame -- ./Make.versions HEAD | grep "${PLATFORM}_NUGET_OS_VERSION=" | sed 's/ .*//')
	COMMIT_DISTANCE=$(git log "$COMMIT..HEAD" --oneline | wc -l | sed -e 's/ //g')
	TOTAL_DISTANCE=$((NUGET_VERSION_COMMIT_DISTANCE_START+COMMIT_DISTANCE))
	printf "${PLATFORM}_NUGET_COMMIT_DISTANCE:=$TOTAL_DISTANCE\\n" >> "$OUTPUT_FILE"
done

STABLE_COMMIT=$(git blame -L '/^[#[:blank:]]*NUGET_RELEASE_BRANCH=/,+1' -- ./Make.config HEAD | sed 's/ .*//')
STABLE_COMMIT_DISTANCE=$(git log "$STABLE_COMMIT..HEAD" --oneline | wc -l | sed -e 's/ //g')
STABLE_TOTAL_DISTANCE=$((STABLE_COMMIT_DISTANCE+NUGET_VERSION_STABLE_COMMIT_DISTANCE_START))

printf "NUGET_STABLE_COMMIT_DISTANCE:=$STABLE_TOTAL_DISTANCE\\n" >> "$OUTPUT_FILE"

# Detect ccache
if which ccache > /dev/null 2>&1; then
	printf "ENABLE_CCACHE=1\n" >> "$OUTPUT_FILE"
	printf "export CCACHE_BASEDIR=$(cd .. && pwd)\n" >> "$OUTPUT_FILE"
	echo "Found ccache on the system, enabling it"
fi

mv "$OUTPUT_FILE" "$OUTPUT"
