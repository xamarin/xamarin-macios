#!/bin/bash -e

find_files() {
    find "$(realpath "$1")" -type f -name "$2" -exec printf "%s," {} +
}

build_dir="$1"
platform="$2"

if [ "$platform" = "macos" ]; then
    modified_platform="mac"
else
    modified_platform="$platform"
fi

root="$(realpath "$build_dir/../../../..")"

# find generated/untracked files
# impt to include, otherwise sourcelink test will fail (don't have a valid source file to link to)
files=$(find_files "$build_dir" "*.g.cs")
files+=$(find_files "$root/src/build/common" "*.cs")
files+=$(find_files "$root/src/build/$modified_platform" "AssemblyInfo.cs")
files+=$(find_files "$root/src/build/$modified_platform" "Constants.cs")
files+=$(find_files "$root/src/build/dotnet" "Constants.$platform.generated.cs")
files+=$(find_files "$root/runtime" "*generated.cs") 
files+=$(find_files "$root/src" "MinimumVersions.cs")
files="-embed:${files%,}"
echo $files
