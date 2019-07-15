#!/bin/bash -ex

# This script takes
# First argument: the github token to authenticate with
# Subsequent arguments: the names of each test variation in the test matrix.
#     There is a corresponding environment variable with the result from each test variation.


# Print environment for debugging
env | sort

TOKEN=$1
shift 1
STEPS="$*"

EMOJII="✅"
GH_STATE=success
FILE=commit-comment.md

for STEP in $STEPS; do
  # The environment variable's name is the variation name in uppercase, and special symbols removed (|-_)
  STEPNAME=JOBRESULT$(echo "$STEP" | tr '[:lower:]' '[:upper:]' | sed -e 's/|//g' -e 's/-//g' -e 's/_//g')
  STEPSTATUS=${!STEPNAME}
  if [[ "$STEPSTATUS" == "Succeeded" ]]; then
    STEPEMOJII="✅"
  else
    STEPEMOJII="❌"
    EMOJII="❌"
    GH_STATE=failure
  fi
  echo "* $STEPEMOJII $STEP: $STEPSTATUS" >> "$FILE"
done

printf "%s\n\n" "$EMOJII Status for '$BUILD_DEFINITIONNAME': [$GH_STATE]($AZURE_BUILD_URL)." | cat - "$FILE" > "$FILE.tmp"
mv "$FILE.tmp" "$FILE"

./jenkins/add-commit-comment.sh "--token=$TOKEN" "--hash=$BUILD_SOURCEVERSION" "--file=$FILE"
./jenkins/add-commit-status.sh "--token=$TOKEN" "--hash=$BUILD_SOURCEVERSION" "--state=$GH_STATE" --target-url="$AZURE_BUILD_URL" --description="$BUILD_DEFINITIONNAME" --context="$BUILD_DEFINITIONNAME"
rm -f "$FILE"
