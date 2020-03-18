#!/bin/bash -ex

cd "$(dirname "${BASH_SOURCE[0]}")/.."

make -C msbuild/dotnet pack -j8 -d

mkdir -p ../package/
rm -f ../package/*.nupkg
cp -c msbuild/dotnet/nupkgs/*.nupkg ../package/
