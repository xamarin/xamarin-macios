#!/bin/bash -e

#
# How to run API comparison locally: check out tools/compare-commits.sh
#

# env var should have been defined by the CI
if test -z "$XAM_TOP"; then
    echo "Variable XAM_TOP is missing."
    exit 1
fi

env | sort # FIXME: debug spew

export WORKSPACE="$BUILD_ARTIFACTSTAGINGDIRECTORY"

CHANGE_DETECTION_OUTPUT_DIR="$WORKSPACE/change-detection"
CHANGE_DETECTION_RESULTS_DIR="$CHANGE_DETECTION_OUTPUT_DIR/results"

mkdir -p "$CHANGE_DETECTION_RESULTS_DIR"

cd "$XAM_TOP"

# Figure out the base hash we want to compare against
if [[ $PR_ID ]]; then
    git fetch --no-tags --progress https://github.com/xamarin/xamarin-macios +refs/pull/"$PR_ID"/*:refs/remotes/origin/pr/"$PR_ID"/*

    # Compute the correct base hash to use for comparison by getting the merge base between the target branch and the commit we're building.
    if MERGE_BASE=$(git merge-base "$SYSTEM_PULLREQUEST_SOURCECOMMITID" "refs/remotes/origin/$SYSTEM_PULLREQUEST_TARGETBRANCH"); then
        if test -n "$MERGE_BASE"; then
            echo "Computed merge base: $MERGE_BASE"
            BASE=$MERGE_BASE
        fi
    fi
fi
if test -z "$BASE"; then
	if test -z "$PR_ID"; then
		BASE=HEAD^1
	else
		BASE="origin/pr/$PR_ID/merge^1"
	fi
fi

mkdir -p "$CHANGE_DETECTION_RESULTS_DIR"

# We always want to zip up (and later upload) whatever's in the results directory, so store the exit code here, and then exit with it later.
RC=0
./tools/compare-commits.sh --base="$BASE" "--output-dir=$CHANGE_DETECTION_OUTPUT_DIR" || RC=$?

if test -d "$CHANGE_DETECTION_OUTPUT_DIR"; then
    rm -f "$CHANGE_DETECTION_OUTPUT_DIR/change-detection.zip"
    cd "$CHANGE_DETECTION_OUTPUT_DIR" && zip -9r "change-detection.zip" .
fi

exit $RC
