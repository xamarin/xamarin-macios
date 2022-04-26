#!/bin/bash -e
# set the env vars that are required for the compare to work. XAM_TOP is not set because
# the yaml takes care of that.
export WORKSPACE="$BUILD_ARTIFACTSTAGINGDIRECTORY"
echo "GH PR: $PR_ID"
cd $XAM_TOP

if [[ $PR_ID ]]; then
    git fetch --no-tags --progress https://github.com/xamarin/xamarin-macios +refs/pull/$PR_ID/*:refs/remotes/origin/pr/$PR_ID/*
fi


if ! ./tools/devops/automation/scripts/bash/compare.sh; then
    set +x
    echo "##vso[task.setvariable variable=API_GENERATOR_BUILT;isOutput=true]False"
    set -x
    exit 1
fi
