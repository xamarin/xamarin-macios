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

cd "$XAM_TOP"

DOTNET_NUPKG_DIR=$(make -C tools/devops print-abspath-variable VARIABLE=DOTNET_NUPKG_DIR | grep "^DOTNET_NUPKG_DIR=" | sed -e 's/^DOTNET_NUPKG_DIR=//')

mkdir -p ../package/
rm -f ../package/*.nupkg
cp -c "$DOTNET_NUPKG_DIR"/*.nupkg ../package/

DOTNET_PKG_DIR=$(make -C tools/devops print-abspath-variable VARIABLE=DOTNET_PKG_DIR | grep "^DOTNET_PKG_DIR=" | sed -e 's/^DOTNET_PKG_DIR=//')
make -C dotnet package -j
cp -c "$DOTNET_PKG_DIR"/*.pkg ../package/
cp -c "$DOTNET_PKG_DIR"/*.msi ../package/

# Down from here we publish mlaunch as .nupkg
MACCORE_HASH=$(cd "$MACCORE_TOP" && git log -1 --pretty=%h)

if nuget list -source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-eng/nuget/v3/index.json -AllVersions -Prerelease Microsoft.DotNet.Mlaunch | grep $MACCORE_HASH; then
    echo "Mlaunch revision $MACCORE_HASH is already published as nupkg"
    exit 0
fi

# Package mlaunch as .nupkg
echo "Packaging mlaunch revision $MACCORE_HASH as nupkg..."

MLAUNCH_WORK_DIR="$DOTNET_NUPKG_DIR/mlaunch-staging"
rm -rf "$MLAUNCH_WORK_DIR"
mkdir -p "$MLAUNCH_WORK_DIR/mlaunch/bin"
mkdir -p "$MLAUNCH_WORK_DIR/mlaunch/lib/mlaunch"

DOTNET6=$(make -C tools/devops print-abspath-variable VARIABLE=DOTNET6 | grep "^DOTNET6=" | sed -e 's/^DOTNET6=//')
IOS_DESTDIR=$(make -C tools/devops print-abspath-variable VARIABLE=IOS_DESTDIR | grep "^IOS_DESTDIR=" | sed -e 's/^IOS_DESTDIR=//')
MONOTOUCH_PREFIX=$(make -C tools/devops print-abspath-variable VARIABLE=MONOTOUCH_PREFIX | grep "^MONOTOUCH_PREFIX=" | sed -e 's/^MONOTOUCH_PREFIX=//')

# Copy mlaunch to staging area
cp -r "$MACCORE_TOP/tools/mlaunch/Xamarin.Hosting/Xamarin.Launcher/bin/Debug/mlaunch.app" "$MLAUNCH_WORK_DIR/mlaunch/lib/mlaunch"
cp "$IOS_DESTDIR$MONOTOUCH_PREFIX/bin/mlaunch" "$MLAUNCH_WORK_DIR/mlaunch/bin/"

# Add the .csproj we will use to create the .nupkg
cp "$XAM_TOP/tools/mlaunch/Microsoft.DotNet.Mlaunch.csproj" "$MLAUNCH_WORK_DIR"

# We need to override global.json to use .NET 6.0
cp "$XAM_TOP/global6.json" "$MLAUNCH_WORK_DIR/global.json"

# We have to build from within the dir to respect the global.json
cd "$MLAUNCH_WORK_DIR"
"$DOTNET6" pack --version-suffix "$MACCORE_HASH"

# We store mlaunch NuGet in [build work root]/mlaunch
cd "$XAM_TOP"
mkdir ../mlaunch
cp "$MLAUNCH_WORK_DIR"/*.nupkg ../mlaunch/
