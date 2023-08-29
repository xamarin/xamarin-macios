#!/bin/bash -eux

set -o pipefail
IFS=$'\n\t '

FILE=$(pwd)/tmp.txt

make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE=DOTNET_PLATFORMS
DOTNET_PLATFORMS=$(cat "$FILE")

make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE=ALL_DOTNET_PLATFORMS
ALL_DOTNET_PLATFORMS=$(cat "$FILE")

make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE=ENABLE_DOTNET
ENABLE_DOTNET=$(cat "$FILE")


make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE=INCLUDE_XAMARIN_LEGACY
INCLUDE_XAMARIN_LEGACY=$(cat "$FILE")

make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE=INCLUDE_IOS
INCLUDE_IOS=$(cat "$FILE")

make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE=INCLUDE_TVOS
INCLUDE_TVOS=$(cat "$FILE")

make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE=INCLUDE_WATCH
INCLUDE_WATCH=$(cat "$FILE")

make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE=INCLUDE_MAC
INCLUDE_MAC=$(cat "$FILE")

rm -f "$FILE"

# print it out, so turn off echoing since that confuses Azure DevOps
set +x

echo "##vso[task.setvariable variable=ENABLE_DOTNET;isOutput=true]$ENABLE_DOTNET"
echo "##vso[task.setvariable variable=DOTNET_PLATFORMS;isOutput=true]$DOTNET_PLATFORMS"
DISABLED_DOTNET_PLATFORMS=" $ALL_DOTNET_PLATFORMS "
for platform in $DOTNET_PLATFORMS; do
	PLATFORM_UPPER=$(echo "$platform" | tr '[:lower:]' '[:upper:]')
	echo "##vso[task.setvariable variable=INCLUDE_DOTNET_$PLATFORM_UPPER;isOutput=true]1"
	DISABLED_DOTNET_PLATFORMS=${DISABLED_DOTNET_PLATFORMS/ $platform / }

	VARIABLE="${PLATFORM_UPPER}_NUGET_VERSION_NO_METADATA"
	make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE="$VARIABLE"
	VALUE=$(cat "$FILE")
	echo "##vso[task.setvariable variable=$VARIABLE;isOutput=true]$VALUE"

	VARIABLE="${PLATFORM_UPPER}_NUGET_SDK_NAME"
	make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE="$VARIABLE"
	VALUE=$(cat "$FILE")
	echo "##vso[task.setvariable variable=$VARIABLE;isOutput=true]$VALUE"

	VARIABLE="${PLATFORM_UPPER}_NUGET_REF_NAME"
	make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE="$VARIABLE"
	VALUE=$(cat "$FILE")
	echo "##vso[task.setvariable variable=$VARIABLE;isOutput=true]$VALUE"

	VARIABLE="DOTNET_${PLATFORM_UPPER}_RUNTIME_IDENTIFIERS"
	make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE="$VARIABLE"
	VALUE=$(cat "$FILE")
	echo "##vso[task.setvariable variable=$VARIABLE;isOutput=true]$VALUE"

	RIDS=$VALUE
	for rid in $RIDS; do
		VARIABLE="${rid}_NUGET_RUNTIME_NAME"
		make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value-to-file FILE="$FILE" VARIABLE="$VARIABLE"
		VALUE=$(cat "$FILE")
		echo "##vso[task.setvariable variable=$VARIABLE;isOutput=true]$VALUE"
	done
done
for platform in $DISABLED_DOTNET_PLATFORMS; do
	PLATFORM_UPPER=$(echo "$platform" | tr '[:lower:]' '[:upper:]')
	echo "##vso[task.setvariable variable=INCLUDE_DOTNET_$PLATFORM_UPPER;isOutput=true]"
done

echo "##vso[task.setvariable variable=INCLUDE_XAMARIN_LEGACY;isOutput=true]$INCLUDE_XAMARIN_LEGACY"
echo "##vso[task.setvariable variable=INCLUDE_LEGACY_IOS;isOutput=true]$INCLUDE_IOS"
echo "##vso[task.setvariable variable=INCLUDE_LEGACY_TVOS;isOutput=true]$INCLUDE_TVOS"
echo "##vso[task.setvariable variable=INCLUDE_LEGACY_WATCH;isOutput=true]$INCLUDE_WATCH"
echo "##vso[task.setvariable variable=INCLUDE_LEGACY_MAC;isOutput=true]$INCLUDE_MAC"

set -x
