#!/bin/bash -ex

echo "##vso[task.setvariable variable=TESTS_BOT;isOutput=true]$AGENT_NAME"
options=()

if [[ "$SYSTEM_DEBUG" == "true" ]]; then
  options=(V=1 -w)
fi

if test -z "$makeParallelism"; then
  options=("${options[@]}" -j8)
else
  options=("${options[@]}" -j $makeParallelism)
fi

# shellcheck disable=SC2046
time make all "${options[@]}" IGNORE_SIMULATORS=1
# shellcheck disable=SC2046
time make install "${options[@]}" IGNORE_SIMULATORS=1
