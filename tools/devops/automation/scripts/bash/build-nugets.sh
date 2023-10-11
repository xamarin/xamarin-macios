#!/bin/bash -ex

# env var should have been defined by the CI
if test -z "$XAM_TOP"; then
    echo "Variable XAM_TOP is missing."
    exit 1
fi

cd "$XAM_TOP"

DOTNET_NUPKG_DIR=$(make -C tools/devops print-abspath-variable VARIABLE=DOTNET_NUPKG_DIR | grep "^DOTNET_NUPKG_DIR=" | sed -e 's/^DOTNET_NUPKG_DIR=//')

mkdir -p ../package/
rm -f ../package/*.nupkg
cp -c "$DOTNET_NUPKG_DIR"/*.nupkg ../package/
cp -c "$DOTNET_NUPKG_DIR"/vs-workload.props ../package/
cp -c dotnet/Workloads/SignList.xml ../package/
cp -c dotnet/Workloads/SignList.targets ../package/
cp -c dotnet/Workloads/SignVerifyIgnore.txt ../package/

DOTNET_PKG_DIR=$(make -C tools/devops print-abspath-variable VARIABLE=DOTNET_PKG_DIR | grep "^DOTNET_PKG_DIR=" | sed -e 's/^DOTNET_PKG_DIR=//')
make -C dotnet package -j
cp -c "$DOTNET_PKG_DIR"/*.pkg ../package/ || true
cp -c "$DOTNET_PKG_DIR"/*.msi ../package/ || true
cp -c "$DOTNET_PKG_DIR"/*.zip ../package/ || true
