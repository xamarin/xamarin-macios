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

if test -z "$PR_ID"; then
    PR_ID=$SYSTEM_PULLREQUEST_PULLREQUESTNUMBER
fi

mkdir -p "$CHANGE_DETECTION_RESULTS_DIR"

# We always want to zip up (and later upload) whatever's in the results directory, so store the exit code here, and then exit with it later.
RC=0
if [[ $PR_ID ]]; then
    ./tools/compare-commits.sh "--output-dir=$CHANGE_DETECTION_OUTPUT_DIR" --pull-request="$PR_ID"|| RC=$?
else
    ./tools/compare-commits.sh "--output-dir=$CHANGE_DETECTION_OUTPUT_DIR" || RC=$?
fi

if test -d "$CHANGE_DETECTION_OUTPUT_DIR"; then
    rm -f "$CHANGE_DETECTION_OUTPUT_DIR/change-detection.zip"
    cd "$CHANGE_DETECTION_OUTPUT_DIR" && zip -9r "change-detection.zip" .
fi

exit $RC
