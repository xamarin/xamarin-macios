#!/bin/bash -ex

DOTNET_NUPKG_DIR=$(make print-abspath-variable VARIABLE=DOTNET_NUPKG_DIR)

cd "$(dirname "${BASH_SOURCE[0]}")/.."

mkdir -p ../package/
rm -f ../package/*.nupkg
cp -c "$DOTNET_NUPKG_DIR"/*.nupkg ../package/
