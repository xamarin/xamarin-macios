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
IOS_DESTDIR=$(make -C tools/devops print-abspath-variable VARIABLE=IOS_DESTDIR | grep "^IOS_DESTDIR=" | sed -e 's/^IOS_DESTDIR=//')
MONOTOUCH_PREFIX=$(make -C tools/devops print-abspath-variable VARIABLE=MONOTOUCH_PREFIX | grep "^MONOTOUCH_PREFIX=" | sed -e 's/^MONOTOUCH_PREFIX=//')

mkdir -p ../package/
rm -f ../package/*.nupkg
cp -c "$DOTNET_NUPKG_DIR"/*.nupkg ../package/

DOTNET_PKG_DIR=$(make -C tools/devops print-abspath-variable VARIABLE=DOTNET_PKG_DIR | grep "^DOTNET_PKG_DIR=" | sed -e 's/^DOTNET_PKG_DIR=//')
make -C dotnet package -j
cp -c "$DOTNET_PKG_DIR"/*.pkg ../package/
cp -c "$DOTNET_PKG_DIR"/*.msi ../package/

MACCORE_HASH=$(cd "$MACCORE_TOP" && git log -1 --pretty=%h)

if nuget list -source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-eng/nuget/v3/index.json -AllVersions -Prerelease Microsoft.DotNet.Mlaunch | grep $MACCORE_HASH; then
    echo "Mlaunch revision $MACCORE_HASH is already published as nupkg"
    exit 0
fi

# Package mlaunch as .nupkg
echo "Packaging mlaunch revision $MACCORE_HASH as nupkg..."

MLAUNCH_WORK_DIR="$DOTNET_NUPKG_DIR/mlaunch-staging"
rm -rf "$MLAUNCH_WORK_DIR/mlaunch"
mkdir -p "$MLAUNCH_WORK_DIR/mlaunch/lib/mlaunch"

DOTNET6=$(make -C tools/devops print-abspath-variable VARIABLE=DOTNET6 | grep "^DOTNET6=" | sed -e 's/^DOTNET6=//')

cp -r "$MACCORE_TOP/tools/mlaunch/Xamarin.Hosting/Xamarin.Launcher/bin/Debug/mlaunch.app" "$MLAUNCH_WORK_DIR/mlaunch/lib/mlaunch"
cp "$XAM_TOP/tools/mlaunch/Microsoft.DotNet.Mlaunch.csproj" "$MLAUNCH_WORK_DIR"
cp "$XAM_TOP/global6.json" "$MLAUNCH_WORK_DIR/global.json"
cp -r "$IOS_DESTDIR$MONOTOUCH_PREFIX/bin" "$MLAUNCH_WORK_DIR/mlaunch"

cd "$MLAUNCH_WORK_DIR"
"$DOTNET6" pack --version-suffix "$MACCORE_HASH"

cd "$XAM_TOP"

# TODO - Remove
mkdir ../mlaunch
cp "$MLAUNCH_WORK_DIR"/*.nupkg ../mlaunch/

stat $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mlaunch