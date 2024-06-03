#!/bin/bash -eu

env | sort

set -o pipefail
IFS=$'\n\t '

FILE=$(pwd)/tmp.txt

make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE=MONO_IOS_FILENAME
MONO_IOS_FILENAME=$(cat "$FILE")
MONO_IOS_FILENAME=$(basename "$MONO_IOS_FILENAME" ".7z")

make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE=MONO_MAC_FILENAME
MONO_MAC_FILENAME=$(cat "$FILE")
MONO_MAC_FILENAME=$(basename "$MONO_MAC_FILENAME" ".7z")

make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE=MONO_MACCATALYST_FILENAME
MONO_MACCATALYST_FILENAME=$(cat "$FILE")
MONO_MACCATALYST_FILENAME=$(basename "$MONO_MACCATALYST_FILENAME" ".7z")

# allow the rest of the build use the values
echo "##vso[task.setvariable variable=MONO_IOS_FILENAME;]$MONO_IOS_FILENAME"
echo "##vso[task.setvariable variable=MONO_MAC_FILENAME;]$MONO_MAC_FILENAME"
echo "##vso[task.setvariable variable=MONO_MACCATALYST_FILENAME;]$MONO_MACCATALYST_FILENAME"

rm -f "$FILE"
