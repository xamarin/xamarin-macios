#!/bin/bash -eux

if [[ $(sysctl -n hw.optional.arm64 2>/dev/null) != '1' ]]; then
  echo "Not running on arm64, nothing to do"
  exit 0
fi

# CodeQL inserts a dylib (using DYLD_INSERT_LIBRARIES) that causes Mono to crash.
# https://twcdot.visualstudio.com/Data/_workitems/edit/116722
# So unset DYLD_INSERT_LIBRARIES.
set -x
echo "##vso[task.setvariable variable=DYLD_INSERT_LIBRARIES]"
set +x
