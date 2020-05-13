#!/bin/bash -ex

cd "$(dirname "${BASH_SOURCE[0]}")/.."

mkdir -p ../package/
rm -f ../package/*.nupkg
cp -c dotnet/nupkgs/*.nupkg ../package/
