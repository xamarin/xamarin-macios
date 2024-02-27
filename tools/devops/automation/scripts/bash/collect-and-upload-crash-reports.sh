#!/bin/bash -ex

if test -z "$SYSTEM_DEFAULTWORKINGDIRECTORY"; then
  SYSTEM_DEFAULTWORKINGDIRECTORY=$(pwd)
fi

if ! test -d "$HOME/Library/Logs/DiagnosticReports"; then
  echo "No crash report directory found" # nothing to do
elif [[ "$(find "$HOME/Library/Logs/DiagnosticReports" -type f | wc -l)" -eq 0 ]]; then
  echo "No crash reports found"  # nothing to do
else
  if test -n "$MACIOS_UPLOAD_PREFIX"; then
    FILENAME="${MACIOS_UPLOAD_PREFIX}-"
  elif test -n "$MACIOS_TEST_PREFIX"; then
    FILENAME="$FILENAME${MACIOS_TEST_PREFIX}-"
  fi
  FILENAME="${FILENAME}crash-reports-$SYSTEM_STAGEDISPLAYNAME-$SYSTEM_JOBATTEMPT"
  zip -9rj "$SYSTEM_DEFAULTWORKINGDIRECTORY/$FILENAME.zip" "$HOME/Library/Logs/DiagnosticReports"
  if test -f "$SYSTEM_DEFAULTWORKINGDIRECTORY/$FILENAME.zip"; then
    set +x
    echo "##vso[artifact.upload containerfolder=CrashReports;artifactname=CrashReports]$SYSTEM_DEFAULTWORKINGDIRECTORY/$FILENAME.zip"
    set -x
  fi
fi
