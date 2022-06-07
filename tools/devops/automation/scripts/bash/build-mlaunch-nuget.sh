#!/bin/bash -ex

# env var should have been defined by the CI
if test -z "$XAM_TOP"; then
    echo "Variable XAM_TOP is missing"
    exit 1
fi

if test -z "$MACCORE_TOP"; then
    echo "Variable MACCORE_TOP is missing"
    exit 1
fi

BUILD_NUMBER=$1
if test -z "$BUILD_NUMBER"; then
    echo "Please supply build number as the first argument"
    exit 1
fi

cd "$XAM_TOP"

MACCORE_HASH=$(cd "$MACCORE_TOP" && git log -1 --pretty=%h)

if nuget list -source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-eng/nuget/v3/index.json -AllVersions -Prerelease Microsoft.DotNet.Mlaunch | grep $MACCORE_HASH; then
    echo "Mlaunch revision $MACCORE_HASH is already published as nupkg"
    exit 0
fi

# Package mlaunch as .nupkg
echo "Packaging mlaunch revision $MACCORE_HASH as nupkg..."

DOTNET_NUPKG_DIR=$(make -C tools/devops print-abspath-variable VARIABLE=DOTNET_NUPKG_DIR | grep "^DOTNET_NUPKG_DIR=" | sed -e 's/^DOTNET_NUPKG_DIR=//')
MLAUNCH_WORK_DIR="$DOTNET_NUPKG_DIR/mlaunch-staging"

rm -rf "$MLAUNCH_WORK_DIR"
mkdir -p "$MLAUNCH_WORK_DIR/mlaunch/bin"
mkdir -p "$MLAUNCH_WORK_DIR/mlaunch/lib/mlaunch"

DOTNET=$(make -C tools/devops print-abspath-variable VARIABLE=DOTNET | grep "^DOTNET=" | sed -e 's/^DOTNET=//')
IOS_DESTDIR=$(make -C tools/devops print-abspath-variable VARIABLE=IOS_DESTDIR | grep "^IOS_DESTDIR=" | sed -e 's/^IOS_DESTDIR=//')
MONOTOUCH_PREFIX=$(make -C tools/devops print-abspath-variable VARIABLE=MONOTOUCH_PREFIX | grep "^MONOTOUCH_PREFIX=" | sed -e 's/^MONOTOUCH_PREFIX=//')

# Copy mlaunch to staging area
cp -c -r "$MACCORE_TOP/tools/mlaunch/Xamarin.Hosting/Xamarin.Launcher/bin/Debug/mlaunch.app" "$MLAUNCH_WORK_DIR/mlaunch/lib/mlaunch"
cp -c "$IOS_DESTDIR$MONOTOUCH_PREFIX/bin/mlaunch" "$MLAUNCH_WORK_DIR/mlaunch/bin/"

# Add the .csproj we will use to create the .nupkg
cp -c -r "$XAM_TOP"/tools/mlaunch/nupkg/* "$MLAUNCH_WORK_DIR"

# We need to override global.json to use .NET 6.0
cp -c "$XAM_TOP/global.json" "$MLAUNCH_WORK_DIR/global.json"

# Version calculation
XCODE_VERSION=$(grep XCODE_VERSION= "$XAM_TOP/Make.config" | sed 's/.*=//')

# Build number is in the format yyyymmdd.r e.g. 20210610.4
BUILD_NUMBER_YY=${BUILD_NUMBER:2:2}
BUILD_NUMBER_MM=${BUILD_NUMBER:4:2}
BUILD_NUMBER_DD=${BUILD_NUMBER:6:2}
BUILD_NUMBER_R=${BUILD_NUMBER:9}

VERSION="$(expr $BUILD_NUMBER_YY \* 1000 + $BUILD_NUMBER_MM \* 50 + $BUILD_NUMBER_DD).$BUILD_NUMBER_R"

# We have to build from within the dir to respect the global.json
cd "$MLAUNCH_WORK_DIR"
"$DOTNET" pack --version-suffix "$VERSION.$MACCORE_HASH" /p:VersionPrefix=$XCODE_VERSION

# We store mlaunch NuGet in [build work root]/mlaunch
cd "$XAM_TOP"
mkdir ../mlaunch
cp -c "$MLAUNCH_WORK_DIR"/*.nupkg ../mlaunch/
