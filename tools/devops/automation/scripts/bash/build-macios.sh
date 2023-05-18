#!/bin/bash -ex

echo "##vso[task.setvariable variable=TESTS_BOT;isOutput=true]$AGENT_NAME"
MAKE_FLAGS=""

if [[ "$SYSTEM_DEBUG" == "true" ]]; then
  MAKE_FLAGS="V=1 -w"
fi

if test -z "$makeParallelism"; then
  makeParallelism=8
fi

time make all -j$makeParallelism $MAKE_FLAGS IGNORE_SIMULATORS=1
time make install -j$makeParallelism $MAKE_FLAGS IGNORE_SIMULATORS=1
