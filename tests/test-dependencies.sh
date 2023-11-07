#!/bin/bash -ex

cd "$(dirname "$0")"
./system-dependencies.sh --provision-mono --ignore-autotools --ignore-xamarin-studio --ignore-xcode --ignore-osx --ignore-cmake --ignore-dotnet --ignore-shellcheck --ignore-yamllint
