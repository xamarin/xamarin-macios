#!/bin/bash -eux

# CodeQL inserts a dylib (using DYLD_INSERT_LIBRARIES) that causes Mono to crash.
# https://twcdot.visualstudio.com/Data/_workitems/edit/116722
# https://dev.azure.com/twcdot/Data/_workitems/edit/137330
# So unset DYLD_INSERT_LIBRARIES.
set -x
echo "##vso[task.setvariable variable=DYLD_INSERT_LIBRARIES]"
set +x
