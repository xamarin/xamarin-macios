#!/bin/bash -ex

cd "$(dirname "${BASH_SOURCE[0]}")/.."

DOTNET_NUPKG_DIR=$(make -C jenkins print-abspath-variable VARIABLE=DOTNET_NUPKG_DIR | grep "^DOTNET_NUPKG_DIR=" | sed -e 's/^DOTNET_NUPKG_DIR=//')

mkdir -p ../package/
rm -f ../package/*.nupkg
cp -c "$DOTNET_NUPKG_DIR"/*.nupkg ../package/
