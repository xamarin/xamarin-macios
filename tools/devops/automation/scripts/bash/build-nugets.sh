#!/bin/bash -ex

# env var should have been defined by the CI
if test -z "$XAM_TOP"; then
    echo "Variable XAM_TOP is missing."
    exit 1
fi

if test -z "$MACCORE_TOP"; then
    echo "Variable MACCORE_TOP is missing."
    exit 1
fi

cd $XAM_TOP

DOTNET_NUPKG_DIR=$(make -C tools/devops print-abspath-variable VARIABLE=DOTNET_NUPKG_DIR | grep "^DOTNET_NUPKG_DIR=" | sed -e 's/^DOTNET_NUPKG_DIR=//')

mkdir -p ../package/
rm -f ../package/*.nupkg
cp -c "$DOTNET_NUPKG_DIR"/*.nupkg ../package/

DOTNET_PKG_DIR=$(make -C tools/devops print-abspath-variable VARIABLE=DOTNET_PKG_DIR | grep "^DOTNET_PKG_DIR=" | sed -e 's/^DOTNET_PKG_DIR=//')
make -C dotnet package -j
cp -c "$DOTNET_PKG_DIR"/*.pkg ../package/
cp -c "$DOTNET_PKG_DIR"/*.msi ../package/

# Package mlaunch as .nupkg
echo "Packaging mlaunch..."
env

MLAUNCH_WORK_DIR="$DOTNET_NUPKG_DIR/mlaunch-staging"

mkdir "$MLAUNCH_WORK_DIR"

DOTNET6=$(make -C tools/devops print-abspath-variable VARIABLE=DOTNET6 | grep "^DOTNET6=" | sed -e 's/^DOTNET6=//')

echo ".NET 6 SDK is at $DOTNET6" # TODO Remove

MACCORE_HASH:=$(shell cd "$MACCORE_TOP" && git log -1 --pretty=%h)

echo "&&&&& MACCORE_HASH is $MACCORE_HASH"

cp -rv "$MACCORE_TOP/tools/mlaunch/Xamarin.Hosting/Xamarin.Launcher/bin/Debug/mlaunch.app" "$MLAUNCH_WORK_DIR"
cp -v "$XAM_TOP/tools/mlaunch/Microsoft.DotNet.Mlaunch.csproj" "$MLAUNCH_WORK_DIR"

"$DOTNET6" pack "$MLAUNCH_WORK_DIR/Microsoft.DotNet.Mlaunch.csproj" --version-suffix "$MACCORE_HASH"
