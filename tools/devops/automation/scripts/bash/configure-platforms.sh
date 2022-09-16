#!/bin/bash -eux

set -o pipefail
IFS=$'\n\t '

DOTNET_PLATFORMS=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value VARIABLE=DOTNET_PLATFORMS)
ALL_DOTNET_PLATFORMS=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value VARIABLE=ALL_DOTNET_PLATFORMS)
ENABLE_DOTNET=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value VARIABLE=ENABLE_DOTNET)

INCLUDE_XAMARIN_LEGACY=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value VARIABLE=INCLUDE_XAMARIN_LEGACY)
INCLUDE_IOS=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value VARIABLE=INCLUDE_IOS)
INCLUDE_TVOS=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value VARIABLE=INCLUDE_TVOS)
INCLUDE_WATCH=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value VARIABLE=INCLUDE_WATCH)
INCLUDE_MAC=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable-value VARIABLE=INCLUDE_MAC)

# print it out, so turn off echoing since that confuses Azure DevOps
set +x

echo "##vso[task.setvariable variable=ENABLE_DOTNET;isOutput=true]$ENABLE_DOTNET"
echo "##vso[task.setvariable variable=DOTNET_PLATFORMS;isOutput=true]$DOTNET_PLATFORMS"
DISABLED_DOTNET_PLATFORMS=" $ALL_DOTNET_PLATFORMS "
for platform in $DOTNET_PLATFORMS; do
	PLATFORM_UPPER=$(echo "$platform" | tr '[:lower:]' '[:upper:]')
	echo "##vso[task.setvariable variable=INCLUDE_DOTNET_$PLATFORM_UPPER;isOutput=true]1"
	DISABLED_DOTNET_PLATFORMS=${DISABLED_DOTNET_PLATFORMS/ $platform / }
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
