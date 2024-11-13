#!/bin/bash -ex

cd "$(dirname "$0")"
./system-dependencies.sh --provision-mono --ignore-xamarin-studio --ignore-xcode --ignore-osx --ignore-dotnet --ignore-shellcheck --ignore-yamllint
