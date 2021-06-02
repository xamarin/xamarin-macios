#!/bin/bash -e
#
# How to run API comparison locally:
#
# 1. To compare a specific commit to the previous one, just run this script (./compare.sh).
# 2. Replicating what happens for a specific PR is a bit more complicated:
#    a. Fetch the source code for that PR, using "git fetch --no-tags --progress https://github.com/xamarin/xamarin-macios +refs/pull/<PR>/*:refs/remotes/origin/pr/<PR>/*",
#       so for example this for PR #4614:
#
#            git fetch --no-tags --progress https://github.com/xamarin/xamarin-macios +refs/pull/4614/*:refs/remotes/origin/pr/4614/*
#
#    b. Checkout the commit you want to compare (typically the branch you created the PR from if it's your own PR).
#       i) This isn't 100% accurate, since GitHub will build a merged version
#          of your branch + the target branch. If you want to test _exactly_
#          the same code as GitHub, checkout the 'origin/pr/<merge>/merge'
#          branch the previous git fetch command created.
#
#    c. Execute 'ghprbPullId=4614 ./compare.sh'
#

# set needed env vars, call the actual script
cd "$(dirname "${BASH_SOURCE[0]}")/../../../../.."
export WORKSPACE=$(pwd)
export XAM_TOP="$WORKSPACE"

./tools/devops/automation/scripts/bash/compare.sh
