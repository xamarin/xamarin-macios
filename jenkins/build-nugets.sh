#!/bin/bash -ex

cd "$(dirname "${BASH_SOURCE[0]}")/.."

make -C msbuild/dotnet
make -C msbuild/dotnet pack -j
# disable legacy packing for now, it takes a while to execute, produces big packages, and rarely change.
#make -C msbuild/dotnet legacy-pack -j

mkdir -p ../package/
rm -f ../package/*.nupkg
cp -c msbuild/dotnet/nupkgs/*.nupkg ../package/
