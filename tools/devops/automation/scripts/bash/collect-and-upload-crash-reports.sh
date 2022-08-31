#!/bin/bash -ex

env | sort

if test -d "$HOME/Library/Logs/DiagnosticReports"; then
  zip -9r "$SYSTEM_DEFAULTWORKINGDIRECTORY/crash-reports.zip" "$HOME/Library/Logs/DiagnosticReports"
  if test -f "$SYSTEM_DEFAULTWORKINGDIRECTORY/crash-reports.zip"; then
    set +x
    echo "##vso[artifact.upload containerfolder=CrashReports;artifactname=crash-reports-$SYSTEM_STAGEDISPLAYNAME-$BUILD_BUILDID-$SYSTEM_JOBATTEMPT)]$SYSTEM_DEFAULTWORKINGDIRECTORY/crash-reports.zip"
    set -x
  fi
fi
