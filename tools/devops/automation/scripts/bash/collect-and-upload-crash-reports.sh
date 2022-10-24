#!/bin/bash -ex

if test -z "$SYSTEM_DEFAULTWORKINGDIRECTORY"; then
  SYSTEM_DEFAULTWORKINGDIRECTORY=$(pwd)
fi

if ! test -d "$HOME/Library/Logs/DiagnosticReports"; then
  echo "No crash report directory found" # nothing to do
elif [[ "$(find "$HOME/Library/Logs/DiagnosticReports" -type f | wc -l)" -eq 0 ]]; then
  echo "No crash reports found"  # nothing to do
else
  zip -9rj "$SYSTEM_DEFAULTWORKINGDIRECTORY/crash-reports.zip" "$HOME/Library/Logs/DiagnosticReports"
  if test -f "$SYSTEM_DEFAULTWORKINGDIRECTORY/crash-reports.zip"; then
    set +x
    echo "##vso[artifact.upload containerfolder=CrashReports;artifactname=crash-reports-$SYSTEM_STAGEDISPLAYNAME-$BUILD_BUILDID-$SYSTEM_JOBATTEMPT]$SYSTEM_DEFAULTWORKINGDIRECTORY/crash-reports.zip"
    set -x
  fi
fi
